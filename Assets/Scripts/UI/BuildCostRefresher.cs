using TMPro;
using UnityEngine;
using System;

public class BuildCostRefresher : MonoBehaviour
{
    [SerializeField] private RoomType roomType;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private string costStart;
    void Start()
    {
        cost = GetComponent<TextMeshProUGUI>();
        costStart = cost.text;
        cost.text = CreateCostText(costStart);
    }

    string CreateCostText(string costStart)
    {
        RoomScript roomScript = FindAnyObjectByType<RoomScript>(); // sorry 😭;

        roomScript.GetPrices(out int asteriumCost, out int honeyCost, out int astroluminiteCost, ConvertResourcesToRoomScriptResources(roomType));

        string costString = (roomType == RoomType.Elevator) ? $"Ресурсы: {asteriumCost} астерия" : $"Ресурсы: {asteriumCost} астерия, {honeyCost} энергомеда, {astroluminiteCost} астролюминита\n";

        return costString + costStart[costStart.IndexOf("Время")..];
    }

    private RoomScript.Resources ConvertResourcesToRoomScriptResources(RoomType resourceType)
    {
        return resourceType switch
        {
            RoomType.Energohoney => RoomScript.Resources.Energohoney,
            RoomType.Asterium    => RoomScript.Resources.Asteriy,
            RoomType.Cosmodrome  => RoomScript.Resources.Cosmodrome,
            RoomType.Bed         => RoomScript.Resources.Bed,
            RoomType.Build       => RoomScript.Resources.Build,
            RoomType.Supply      => RoomScript.Resources.Supply,
            RoomType.Research    => RoomScript.Resources.Research,
            _ => throw new ArgumentException(nameof(ConvertResourcesToRoomScriptResources), "Unknown room or it's elevator"),
        };
    }
}
