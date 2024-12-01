using System;
using TMPro;
using UnityEngine;

public class RoomStatusController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI durabilityShow;
    [SerializeField] private TextMeshProUGUI stateShow;
    [SerializeField] private GameObject obj;

    public void Init(GameObject obj, string name, float durability, RoomScript.Status status)
    {
        this.obj = obj;
        roomName.text = name;
        durabilityShow.text = DurabilityToText(durability);
        stateShow.text = StatusToText(status);
    }

    private string DurabilityToText(float durability)
    {
        return Convert.ToString((int)(durability * 100)) + "%";
    }

    private string StatusToText(RoomScript.Status status)
    {
        switch (status)
        {
            case RoomScript.Status.Destroyed:
                return "Разрушено";
            case RoomScript.Status.Busy:
                return "Работает";
            case RoomScript.Status.Free:
                return "Простаивает";
            default:
                return "Неизвестный статус";
        }
    }

    public void UpdateDurability(float durability)
    {
        durabilityShow.text = DurabilityToText(durability);
    }

    public void UpdateStatus(RoomScript.Status status)
    {
        stateShow.text = StatusToText(status);
    }

    public void MoveToObject()
    {
        Camera.main.GetComponent<CameraController>().MoveToPoint(obj.transform.position);
    }
}
