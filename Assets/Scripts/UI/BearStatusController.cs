using UnityEngine;
using TMPro;
using System;

public class BearStatusController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bearName;
    [SerializeField] private TextMeshProUGUI levelShow;
    [SerializeField] private TextMeshProUGUI jobShow;
    [SerializeField] private TextMeshProUGUI stateShow;
    [SerializeField] private GameObject obj;

    public void Init(GameObject obj, string name, int level, Qualification job, bool isBearBusy)
    {
        this.obj = obj;
        bearName.text = name;
        levelShow.text = Convert.ToString(level);
        jobShow.text = QualificationToText(job);
        stateShow.text = isBearBusy ? "Работает" : "Не занят";
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

    public void UpdateLevel(int level)
    {
        levelShow.text = Convert.ToString(level);
    }

    public void UpdateState(bool isBearBusy)
    {
        stateShow.text = isBearBusy ? "Работает" : "Не занят";
    }

    public void MoveToObject()
    {
        Camera.main.GetComponent<CameraController>().MoveToPoint(obj.transform.position);
        GameManager.Instance.ClickedGameObject(obj);
    }
}
