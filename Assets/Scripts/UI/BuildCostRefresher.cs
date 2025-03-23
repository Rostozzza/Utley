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
        roomType = GetComponentInParent<ChoiceController>().GetRoomType();
        cost = GetComponent<TextMeshProUGUI>();
        costStart = cost.text;
        cost.text = CreateCostText(costStart);
    }

    string CreateCostText(string costStart)
    {
        int asteriumCost = 0;
        int honeyCost = 0;
        int astroluminiteCost = 0;

        RoomScript roomScript = FindAnyObjectByType<RoomScript>(); // sorry 😭;

        if (roomType != RoomType.Elevator) roomScript.GetPrices(out asteriumCost, out honeyCost, out astroluminiteCost, ConvertResourcesToRoomScriptResources(roomType));
        else
        {
            asteriumCost = ValuesHolder.RoomsBuildPrice[RoomType.Elevator][ResourceType.AsteriumPrice];
            honeyCost = ValuesHolder.RoomsBuildPrice[RoomType.Elevator][ResourceType.EnergohoneyPrice];
            astroluminiteCost = ValuesHolder.RoomsBuildPrice[RoomType.Elevator][ResourceType.AstroluminitePrice];
        }
        string costString = (roomType == RoomType.Elevator) ? $"Ресурсы:\n{asteriumCost} астерия <sprite=0>\n" : $"{(asteriumCost > 0 ? $"Ресурсы:\n{asteriumCost} астерия <sprite=0>\n" : "")}{(honeyCost > 0 ? $"{honeyCost} энергомеда <sprite=1>\n" : "")}{(astroluminiteCost > 0 ? $"{astroluminiteCost} астролюминита <sprite=2>\n" : "")}"; //god left us

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
