using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BindSelector : MonoBehaviour
{
    [SerializeField] private ActionKeys action; // from editor;
    [SerializeField] private KeyCode keyCode; // dynamic;
    [SerializeField] private TextMeshProUGUI textAction;
    [SerializeField] private TextMeshProUGUI textKeyCode;

    private void Start()
    {
        keyCode = InputController.GetKeyDict()[action];
        InitText(action, keyCode);
    }

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
            "Mouse0" => "L.Mouse",
            "Mouse1" => "R.Mouse",

            _ => text,
        };
    }

    private string ActionToText(ActionKeys action)
    {
        string toReturn = action switch
        {
            ActionKeys.None => "Ничего",
            ActionKeys.Pause => "Вызов паузы",
            ActionKeys.Quit => "Назад",
            _ => "Имя не найдено",
        };
        return toReturn;
    }

    public void ChooseBind()
    {
        StartCoroutine(BindChooser());
    }

    public void EraseBind()
    {
        BindChooser(InputController.GetDefaultKeyDict()[action]);
    }

    private IEnumerator BindChooser()
    {
        yield return StartCoroutine(GetPressedKey());
        SendBindToController(action, keyCode);
        InitText(action, keyCode);
    }

    private void BindChooser(KeyCode keyCode)
    {
        this.keyCode = keyCode;
        SendBindToController(action, keyCode);
        InitText(action, keyCode);
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

    public void SetAction(ActionKeys action)
    {
        this.action = action;
    }
}
