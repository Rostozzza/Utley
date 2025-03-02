using System.Collections.Generic;

//public class SeasonsAndEffects
//{
//    public Dictionary<string, float> CalmSeason { get; set; }
//    public Dictionary<string, float> StormSeason { get; set; }
//    public Dictionary<string, Dictionary<string, float>> FreezeSeason { get; set; }
//    public Dictionary<string, float> TideSeason { get; set; }
//}
//
//public class EnergohoneyConsumption
//{
//    public Dictionary<string, float> RoomLvl1 { get; set; }
//    public Dictionary<string, float> RoomLvl2 { get; set; }
//    public Dictionary<string, float> RoomLvl3 { get; set; }
//    public Dictionary<string, float> RoomFreeze { get; set; }
//}
//
//public class ExperienceGain
//{
//    public Dictionary<int, Dictionary<string, float>> Level {get; set;}
//}
//
//public class StrengthLoss
//{
//    public Dictionary<int, Dictionary<string, float>> Season {get; set;}
//}
//
//public class AsteriumCosts
//{
//    public int Building { get; set; }
//    public int UpgradeTo2Lvl { get; set; }
//    public int UpgradeTo3Lvl { get; set; }
//    public int Repair { get; set; }
//}
//
//public class InteracionTime
//{
//    
//}

public class Constants
{
    public int StartAstroluminite { get; set; }
    public int StartAsterium { get; set; }
    public float StartEnergohoney { get; set; }
    public float InteractionSpeedMultiplyerByLevel { get; set; }
    public float InteractionSpeedMultiplyerByGrade { get; set; }
    public float MaxTemperature { get; set; }
    public float MinTemperature { get; set; }
    public float StandartInteractionTime { get; set; }
    public float InteracionTimeMultiplyerByCorrectJob { get; set; }
    public float DurationLoss { get; set; }
    public float RepairSpeed { get; set; }
    public int AstroluminiteAmountByOneInteraction { get; set; }
    public float EnergohoneyAmountByOneInteraction { get; set; }
    public int AsteriumAmountByOneInteraction { get; set; }
    public float GameDuration { get; set; }
    public float CycleDuration { get; set; }
}