using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class BearStatusController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bearName;
    [SerializeField] private TextMeshProUGUI levelShow;
    [SerializeField] private TextMeshProUGUI jobShow;
    [SerializeField] private TextMeshProUGUI stateShow;
    [SerializeField] private GameObject obj;
    [SerializeField] private Image avatar;
    [SerializeField] private bool isSelected = false;
    [SerializeField] private TextMeshProUGUI workStrShow;
    [SerializeField] private Slider LvlSlider;
    [SerializeField] private Image loveWorkImage;
    [SerializeField] private Sprite[] UpArrow;

    public void Init(GameObject obj, string name, float level, Qualification job, bool isBearBusy, string workText, Sprite avatar)
    {
        this.obj = obj;
        bearName.text = name;
        levelShow.text = Convert.ToString((int)level);
        jobShow.text = QualificationToText(job);
        //stateShow.text = isBearBusy ? "Работает" : "Не занят";
        workStrShow.text = workText;
        this.avatar.sprite = avatar;
        LvlSlider.value = level % 1;
        loveWorkImage.sprite = UpArrow[0];
        GameManager.Instance.AddBearToMove(this);
    }

    private string QualificationToText(Qualification job)
    {
        switch (job)
        {
            case Qualification.researcher:
                return "Первопроходец";
            case Qualification.bioengineer:
                return "Биоинженер";
            case Qualification.builder:
                return "Конструктор";
            case Qualification.beekeeper:
                return "Пасечник";
            case Qualification.coder:
                return "Программист";
            case Qualification.creator:
                return "Творец";
            default:
                return "Неизвестно";
        }
    }

    public void UpdateLevel(float level)
    {
        levelShow.text = Convert.ToString((int)level);
        if (level == 5)
        {
            LvlSlider.value = 1;
        }
        else
        {
            LvlSlider.value = level % 1;
        }
    }

    public void UpdateState(bool isBearBusy)
    {
        //stateShow.text = isBearBusy ? "Работает" : "Не занят";
    }

    public void MoveToObject()
    {
        SetSelect(true);
        Camera.main.GetComponent<CameraController>().MoveToPoint(obj.transform.position);
        GameManager.Instance.ClickedGameObject(obj);
    }

    public GameObject GetBearObj()
    {
        return obj;
    }

    public void SetSelect(bool set)
    {
        isSelected = set;
        if (set)
        {
            avatar.color = Color.white;
        }
        else
        {
            avatar.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }

    public void UpdateWorkStr(string str)
    {
        if (str == "Не занят")
        {
            workStrShow.color = Color.white;
        }
        else
        {
            workStrShow.color = (Color.red + Color.yellow) / 2f;
        }
        workStrShow.text = str;
    }

    public void UpdateLoveWork(bool update)
    {
        loveWorkImage.sprite = UpArrow[update ? 1 : 0];
    }
}
