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
    public int StartPrototype { get; set; }
    public int StartUrsowaks { get; set; }
    public int PrototypeAmountByOneInteraction { get; set; }
    public int UrsowaksAmountByOneInteraction { get; set; }
    public float StandartInteractionTimeAsteriumComplex { get; set; }
    public float CycleModifier { get; set; }
    public float BuyBears { get; set; }
    public float BuyHoney { get; set; }
    public float BuyTime { get; set; }
    public float BuyTemperatureBoost { get; set; }
    public float BuyAsterium { get; set; }
    public float SellHoney { get; set; }
    public float SellAsterium { get; set; }
    public float SellAstroluminite { get; set; }
    public float SellPrototype { get; set; }
    public float SellUrsowaks { get; set; }

    public Dictionary<RoomType, Dictionary<ResourceType, int>> RoomsBuildPrice { get; set; }
    
    //public int ElevatorAsteriumPrice { get; set; }
    //public int ElevatorEnergohoneyPrice { get; set; }
    //public int ElevatorAstroluminitePrice { get; set; }
//
    //public int EnergohoneyAsteriumPrice { get; set; }
    //public int EnergohoneyEnergohoneyPrice { get; set; }
    //public int EnergohoneyAstroluminitePrice { get; set; }
//
    //public int AsteriyAsteriumPrice { get; set; }
    //public int AsteriyEnergohoneyPrice { get; set; }
    //public int AsteriyAstroluminitePrice { get; set; }
//
    //public int CosmodromeAsteriumPrice { get; set; }
    //public int CosmodromeEnergohoneyPrice { get; set; }
    //public int CosmodromeAstroluminitePrice { get; set; }
//
    //public int BedAsteriumPrice { get; set; }
    //public int BedEnergohoneyPrice { get; set; }
    //public int BedAstroluminitePrice { get; set; }
//
    //public int BuildAsteriumPrice { get; set; }
    //public int BuildEnergohoneyPrice { get; set; }
    //public int BuildAstroluminitePrice { get; set; }
//
    //public int SupplyAsteriumPrice { get; set; }
    //public int SupplyEnergohoneyPrice { get; set; }
    //public int SupplyAstroluminitePrice { get; set; }
//
    //public int ResearchAsteriumPrice { get; set; }
    //public int ResearchEnergohoneyPrice { get; set; }
    //public int ResearchAstroluminitePrice { get; set; }
    
    public int RepairCost { get; set; }
    public float DamageByTide { get; set; }
    public float DamageByTideMultiplier { get; set; }
    public float EnergohoneyConsumeMultiplier { get; set; }
    public float EnergohoneyConsumeMultiplierByRoom { get; set; }
}

public enum RoomType
{
    Elevator,
    Energohoney,
    Asterium,
    Cosmodrome,
    Bed,
    Build,
    Supply,
    Research
}

public enum ResourceType
{
    Asterium,
    Energohoney,
    Astroluminite
}