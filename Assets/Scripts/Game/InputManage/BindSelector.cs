using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BindSelector : MonoBehaviour
{
    [SerializeField] private ActionKeys action; // from editor;
    [SerializeField] private KeyCode keyCode; // dynamic;
    [SerializeField] private TextMeshProUGUI textAction;
    [SerializeField] private TextMeshProUGUI textKeyCode;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color changedColor;
    [SerializeField] private Color conflictColor;

    private void Start()
    {
        baseColor = buttonImage.color;
        keyCode = InputController.GetKeyDict()[action];
        InitText(action, keyCode);
        ColorBind();
        InputController.AddBind(this);
    }

    public void InitText() => InitText(action, keyCode);
    private void InitText(ActionKeys action, KeyCode keyCode)
    {
        textAction.text = ActionToText(action);
        textKeyCode.text = MakeReadable(Convert.ToString(keyCode));
    }

    private string MakeReadable(string text)
    {
        text = text.Replace("Left", "L.");
        text = text.Replace("Right", "R.");
        text = text.Replace("Control", "Ctrl");

        text = text.Replace("Alpha", "");
        text = text.Replace("Keypad", "");

        return text switch
        {
            "Escape" => "Esc",
            "Return" => "Enter",
            "Delete" => "Del",
            "Minus" => "-",
            "Equals" => "=",
            "Plus" => "+",
            "Mouse0" => "LMB",
            "Mouse1" => "RMB",
            "Mouse2" => "MMB",

            _ => text,
        };
    }

    private string ActionToText(ActionKeys action)
    {
        string toReturn = action switch
        {
            ActionKeys.None => "Ничего",
            ActionKeys.Pause => "Паузы",
            ActionKeys.Quit => "Назад",
            ActionKeys.Bear1 => "Медведь 1",
            ActionKeys.Bear2 => "Медведь 2",
            ActionKeys.Bear3 => "Медведь 3",
            ActionKeys.Bear4 => "Медведь 4",
            ActionKeys.Bear5 => "Медведь 5",
            ActionKeys.Bear6 => "Медведь 6",
            ActionKeys.OpenShop => "Открыть магазин",
            //ActionKeys.SelectUnitByPointer => "Выбрать медведя",
            ActionKeys.MoveBearToRoom => "Переместить медведя",
            ActionKeys.BuildMode => "Режим строительства",
            ActionKeys.InfoMode => "Режим информации",
            _ => "Имя не найдено",
        };
        return toReturn;
    }

    public void ChooseBindByButton()
    {
        StartCoroutine(BindChooser());
    }

    public void EraseBind()
    {
        BindChoose(InputController.GetDefaultKeyDict()[action], true);
    }

    private IEnumerator BindChooser()
    {
        yield return StartCoroutine(GetPressedKey());
        ColorBind();
        if (!HasConflict(keyCode)) SendBindToController(action, keyCode);
        InitText(action, keyCode);
    }

    private void BindChoose(KeyCode keyCode, bool force)
    {
        this.keyCode = keyCode;
        ColorBind();
        if (force || !HasConflict(keyCode)) SendBindToController(action, keyCode);
        InitText(action, keyCode);
    }

    private void ColorBind()
    {
        buttonImage.color = InputController.GetDefaultKeyDict()[action] == keyCode ? buttonImage.color = baseColor : HasConflict(keyCode) ? buttonImage.color = conflictColor : buttonImage.color = changedColor; // this spaghetti is analogue to lower commented code;
        
        //if (InputController.GetDefaultKeyDict()[action] == keyCode) 
        //{
        //    buttonImage.color = baseColor;
        //}
        //else
        //{
        //    if (HasConflict(keyCode))
        //    {
        //        buttonImage.color = conflictColor;
        //    }
        //    else
        //    {
        //        buttonImage.color = changedColor;
        //    }
        //}
    }

    private bool HasConflict(KeyCode keyCode)
    {
        bool hasAlreadySameValue = false;
        foreach (var pair in InputController.GetKeyDict())
        {
            if (pair.Key != action && pair.Value == keyCode)
            {
                hasAlreadySameValue = true;
                break;
            }
        }
        return hasAlreadySameValue;
    }

    private IEnumerator GetPressedKey()
    {
        while (true)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) // this foreach waits for user any key input;
            {
                if (Input.GetKeyDown(key))
                {
                    keyCode = key;
                    yield break;
                }
            }
            yield return null;
        }
    }

    private void SendBindToController(ActionKeys action, KeyCode keyCode)
    {
        InputController.ReceiveBindFromSelector(action, keyCode);
    }

    public void SolveConflict()
    {
        if (!HasConflict(keyCode)) return;
        keyCode = InputController.keyDict[action];
        InitText();
        ColorBind();
    }

    public void SetAction(ActionKeys action) => this.action = action;
    public ActionKeys GetAction() => action;
}
