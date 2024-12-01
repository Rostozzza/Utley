using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RoomStatusListController : MonoBehaviour
{
    public List<GameObject> rooms;
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform content;
    public RoomStatusController CreateRoomStatus(RoomScript obj)
    {
        GameObject instantiateRoomPanel = Instantiate(panelPrefab, content);
        rooms.Add(instantiateRoomPanel);
        instantiateRoomPanel.GetComponent<RoomStatusController>().Init(ResourcesTypeToName(obj.resource), obj.durability, obj.status);
        return instantiateRoomPanel.GetComponent<RoomStatusController>();
    }

    private string ResourcesTypeToName(RoomScript.Resources resource)
    {
        switch (resource)
        {
            case RoomScript.Resources.Energohoney:
                return "Комплекс выработки энергомеда";
            case RoomScript.Resources.Asteriy:
                return "Комплекс переработки астерия";
            case RoomScript.Resources.Cosmodrome:
                return "Космодром";
            case RoomScript.Resources.Bed:
                return "Жилой комплекс";
            case RoomScript.Resources.Build:
                return "Комплекс строительства";
            case RoomScript.Resources.Supply:
                return "Комплекс снабжения";
            default:
                return "Неизвестный тип";
        }
    }
}
