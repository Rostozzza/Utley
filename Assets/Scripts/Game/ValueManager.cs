using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

public class ValueManager : MonoBehaviour
{
    public Constants model;
    private string path;

    private void Awake()
    {
        TryGetConfig();

        SetValuesHolder();
        Debug.Log(ValuesHolder.GameDuration);
    }

    private void SetValuesHolder()
    {
        ValuesHolder.StartAstroluminite                     = model.StartAstroluminite;
        ValuesHolder.StartAsterium                          = model.StartAsterium;
        ValuesHolder.StartEnergohoney                       = model.StartEnergohoney;
        ValuesHolder.InteractionSpeedMultiplyerByLevel      = model.InteractionSpeedMultiplyerByLevel;
        ValuesHolder.InteractionSpeedMultiplyerByGrade      = model.InteractionSpeedMultiplyerByGrade;
        ValuesHolder.MaxTemperature                         = model.MaxTemperature;
        ValuesHolder.MinTemperature                         = model.MinTemperature;
        ValuesHolder.StandartInteractionTime                = model.StandartInteractionTime;
        ValuesHolder.InteracionTimeMultiplyerByCorrectJob   = model.InteracionTimeMultiplyerByCorrectJob;
        ValuesHolder.DurationLoss                           = model.DurationLoss;
        ValuesHolder.RepairSpeed                            = model.RepairSpeed;
        ValuesHolder.AstroluminiteAmountByOneInteraction    = model.AstroluminiteAmountByOneInteraction;
        ValuesHolder.EnergohoneyAmountByOneInteraction      = model.EnergohoneyAmountByOneInteraction;
        ValuesHolder.AsteriumAmountByOneInteraction         = model.AsteriumAmountByOneInteraction;
        ValuesHolder.GameDuration                           = model.GameDuration;
        ValuesHolder.CycleDuration                          = model.CycleDuration;
        ValuesHolder.StartPrototype                         = model.StartPrototype;
        ValuesHolder.StartUrsowaks                          = model.StartUrsowaks;
        ValuesHolder.PrototypeAmountByOneInteraction        = model.PrototypeAmountByOneInteraction;
        ValuesHolder.UrsowaksAmountByOneInteraction         = model.UrsowaksAmountByOneInteraction;
        ValuesHolder.StandartInteractionTimeAsteriumComplex = model.StandartInteractionTimeAsteriumComplex;
        ValuesHolder.CycleModifier                          = model.CycleModifier;
        ValuesHolder.BuyBears                               = model.BuyBears;
        ValuesHolder.BuyHoney                               = model.BuyHoney;
        ValuesHolder.BuyTime                                = model.BuyTime;
        ValuesHolder.BuyTemperatureBoost                    = model.BuyTemperatureBoost;
        ValuesHolder.BuyAsterium                            = model.BuyAsterium;
        ValuesHolder.SellHoney                              = model.SellHoney;
        ValuesHolder.SellAsterium                           = model.SellAsterium;
        ValuesHolder.SellAstroluminite                      = model.SellAstroluminite;
        ValuesHolder.SellPrototype                          = model.SellPrototype;
        ValuesHolder.SellUrsowaks                           = model.SellUrsowaks;

        ValuesHolder.RoomsBuildPrice                        = model.RoomsBuildPrice;

        //ValuesHolder.ElevatorAsteriumPrice                  = model.ElevatorAsteriumPrice;
        //ValuesHolder.ElevatorEnergohoneyPrice               = model.ElevatorEnergohoneyPrice;
        //ValuesHolder.ElevatorAstroluminitePrice             = model.ElevatorAstroluminitePrice;
        // 
        //ValuesHolder.EnergohoneyAsteriumPrice               = model.EnergohoneyAsteriumPrice;
        //ValuesHolder.EnergohoneyEnergohoneyPrice            = model.EnergohoneyEnergohoneyPrice;
        //ValuesHolder.EnergohoneyAstroluminitePrice          = model.EnergohoneyAstroluminitePrice;
        // 
        //ValuesHolder.AsteriyAsteriumPrice                   = model.AsteriyAsteriumPrice;
        //ValuesHolder.AsteriyEnergohoneyPrice                = model.AsteriyEnergohoneyPrice;
        //ValuesHolder.AsteriyAstroluminitePrice              = model.AsteriyAstroluminitePrice;
        //  
        //ValuesHolder.CosmodromeAsteriumPrice                = model.CosmodromeAsteriumPrice;
        //ValuesHolder.CosmodromeEnergohoneyPrice             = model.CosmodromeEnergohoneyPrice;
        //ValuesHolder.CosmodromeAstroluminitePrice           = model.CosmodromeAstroluminitePrice;
        //  
        //ValuesHolder.BedAsteriumPrice                       = model.BedAsteriumPrice;
        //ValuesHolder.BedEnergohoneyPrice                    = model.BedEnergohoneyPrice;
        //ValuesHolder.BedAstroluminitePrice                  = model.BedAstroluminitePrice;
        //  
        //ValuesHolder.BuildAsteriumPrice                     = model.BuildAsteriumPrice;
        //ValuesHolder.BuildEnergohoneyPrice                  = model.BuildEnergohoneyPrice;
        //ValuesHolder.BuildAstroluminitePrice                = model.BuildAstroluminitePrice;
        //  
        //ValuesHolder.SupplyAsteriumPrice                    = model.SupplyAsteriumPrice;
        //ValuesHolder.SupplyEnergohoneyPrice                 = model.SupplyEnergohoneyPrice;
        //ValuesHolder.SupplyAstroluminitePrice               = model.SupplyAstroluminitePrice;
        //  
        //ValuesHolder.ResearchAsteriumPrice                  = model.ResearchAsteriumPrice;
        //ValuesHolder.ResearchEnergohoneyPrice               = model.ResearchEnergohoneyPrice;
        //ValuesHolder.ResearchAstroluminitePrice             = model.ResearchAstroluminitePrice;
        
        ValuesHolder.RepairCost                             = model.RepairCost;
        ValuesHolder.DamageByTide                           = model.DamageByTide;
        ValuesHolder.DamageByTideMultiplier                 = model.DamageByTideMultiplier;
        ValuesHolder.EnergohoneyConsumeMultiplier           = model.EnergohoneyConsumeMultiplier;
        ValuesHolder.EnergohoneyConsumeMultiplierByRoom     = model.EnergohoneyConsumeMultiplierByRoom;
        ValuesHolder.EnergohoneyConsumeMultiplierByCycle    = model.EnergohoneyConsumeMultiplierByCycle;

        ValuesHolder.EnergohoneyExponent                    = model.EnergohoneyExponent;
    }

    public void TryGetConfig()
    {
        path = Application.isEditor ? Application.dataPath + "/Resources" : path = Directory.GetCurrentDirectory();
        if (!File.Exists(path + "/config.json")) MakeTemplate(path);
        model = JsonConvert.DeserializeObject<Constants>(File.ReadAllText(path + "/config.json"));
    }

    public void MakeTemplate(string path)
    {
        Constants tm = new()
        {
            StartAstroluminite = 6,
            StartAsterium = 20,
            StartEnergohoney = 40.0f,
            InteractionSpeedMultiplyerByLevel = 0.85f,
            InteractionSpeedMultiplyerByGrade = 0.95f,
            MaxTemperature = 20.0f,
            MinTemperature = 25.0f,
            StandartInteractionTime = 23.0f,
            InteracionTimeMultiplyerByCorrectJob = 0.75f,
            DurationLoss = 4.0f,
            RepairSpeed = 3.0f,
            AstroluminiteAmountByOneInteraction = 6,
            EnergohoneyAmountByOneInteraction = 9.0f,
            AsteriumAmountByOneInteraction = 25,
            GameDuration = 960.0f,
            CycleDuration = 240.0f,
            StartPrototype = 0,
            StartUrsowaks = 0,
            PrototypeAmountByOneInteraction = 1,
            UrsowaksAmountByOneInteraction = 1,
            StandartInteractionTimeAsteriumComplex = 40f,
            CycleModifier = 1.0f,
            BuyBears = -7.0f,
            BuyHoney = -0.25f,
            BuyTime = -3.0f,
            BuyTemperatureBoost = -7.0f,
            BuyAsterium = -0.12f,
            SellHoney = 0.2f,
            SellAsterium = 0.1f,
            SellAstroluminite = 0.6f,
            SellPrototype = 3.0f,
            SellUrsowaks = 5.0f,

            RoomsBuildPrice = new()
            {
                { RoomType.Elevator,    new(){ { ResourceType.AsteriumPrice, 10 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 0 }, { ResourceType.RepairAsteriumCost, 15 }, { ResourceType.RepairAstroluminiteCost, 1 } } },
                { RoomType.Energohoney, new(){ { ResourceType.AsteriumPrice, 20 }, { ResourceType.EnergohoneyPrice, 25 }, { ResourceType.AstroluminitePrice, 1 }, { ResourceType.RepairAsteriumCost, 15 }, { ResourceType.RepairAstroluminiteCost, 1 } } },
                { RoomType.Asterium,    new(){ { ResourceType.AsteriumPrice, 30 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 3 }, { ResourceType.RepairAsteriumCost, 15 }, { ResourceType.RepairAstroluminiteCost, 1 } } },
                { RoomType.Cosmodrome,  new(){ { ResourceType.AsteriumPrice, 0 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 0 }, { ResourceType.RepairAsteriumCost, 15 }, { ResourceType.RepairAstroluminiteCost, 1 } } },
                { RoomType.Bed,         new(){ { ResourceType.AsteriumPrice, 25 }, { ResourceType.EnergohoneyPrice, 10 }, { ResourceType.AstroluminitePrice, 0 }, { ResourceType.RepairAsteriumCost, 15 }, { ResourceType.RepairAstroluminiteCost, 1 } } },
                { RoomType.Build,       new(){ { ResourceType.AsteriumPrice, 35 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 3 }, { ResourceType.RepairAsteriumCost, 15 }, { ResourceType.RepairAstroluminiteCost, 1 } } },
                { RoomType.Supply,      new(){ { ResourceType.AsteriumPrice, 30 }, { ResourceType.EnergohoneyPrice, 5 }, { ResourceType.AstroluminitePrice, 2 }, { ResourceType.RepairAsteriumCost, 15 }, { ResourceType.RepairAstroluminiteCost, 1 } } },
                { RoomType.Research,    new(){ { ResourceType.AsteriumPrice, 25 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 1 }, { ResourceType.RepairAsteriumCost, 15 }, { ResourceType.RepairAstroluminiteCost, 1 } } }
            },

            RepairCost = 15,
            DamageByTide = 7.0f,
            DamageByTideMultiplier = 1.0f,
            EnergohoneyConsumeMultiplier = 1.0f,
            EnergohoneyConsumeMultiplierByRoom = 1.0f,
            EnergohoneyConsumeMultiplierByCycle = 9.5f,
            EnergohoneyExponent = 1f,
        };
        //Debug.Log(JsonConvert.SerializeObject(tm, Formatting.Indented));
        File.WriteAllText(path + "/config.json", JsonConvert.SerializeObject(tm, Formatting.Indented));
    }
}

