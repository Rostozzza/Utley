using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomStatusController : MonoBehaviour
{
    [SerializeField] private Image panel;
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI durabilityShow;
    [SerializeField] private TextMeshProUGUI stateShow;
    [SerializeField] private GameObject obj;
    [SerializeField] private Image icon;
    [SerializeField] private List<Sprite> icons;
    private Color defaultColor;
    private string workStr;

    private void Start()
    {
        defaultColor = panel.color;
    }

    public void Init(GameObject obj, string name, float durability, RoomScript.Status status, RoomScript.Resources resource)
    {
        this.obj = obj;
        roomName.text = name;
        durabilityShow.text = DurabilityToText(durability);
        stateShow.text = StatusToText(status);
        icon.sprite = ResourceTypeToSprite(resource);
    }

    private Sprite ResourceTypeToSprite(RoomScript.Resources resource)
    {
        return icons[(int)resource];
    }

    private string DurabilityToText(float durability)
    {
        return Convert.ToString((int)(durability * 100)) + "/100";
    }

    private string StatusToText(RoomScript.Status status)
    {
        switch (status)
        {
            case RoomScript.Status.Destroyed:
                return "Сломан";
            case RoomScript.Status.Busy:
                return WorkStr();
            case RoomScript.Status.Free:
                return "Простаивает";
            default:
                return "Неизвестный статус";
        }
    }

    private string WorkStr()
    {
        return obj.GetComponent<RoomScript>().workStr;
    }

    public void UpdateDurability(float durability)
    {
        durabilityShow.text = DurabilityToText(durability);
    }

    public void UpdateStatus(RoomScript.Status status)
    {
        switch (status)
        {
            case RoomScript.Status.Free:
                panel.color = defaultColor;
                stateShow.color = Color.white;
                durabilityShow.color = Color.white;
                icon.color = Color.white;
                break;
            case RoomScript.Status.Busy:
                panel.color = defaultColor;
                stateShow.color = (Color.red + Color.yellow) / 2f;
                durabilityShow.color = Color.white;
                icon.color = Color.white;
                break;
            case RoomScript.Status.Destroyed:
                panel.color = Color.red;
                stateShow.color = Color.black;
                durabilityShow.color = Color.black;
                icon.color = Color.black;

                break;
        }
        stateShow.text = StatusToText(status);
    }

    public void MoveToObject()
    {
        Camera.main.GetComponent<CameraController>().MoveToPoint(obj.transform.position);
    }
}
