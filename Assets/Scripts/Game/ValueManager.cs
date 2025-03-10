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
        TryGetBalance();

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

        ValuesHolder.ElevatorAsteriumPrice                  = model.ElevatorAsteriumPrice;
        ValuesHolder.ElevatorEnergohoneyPrice               = model.ElevatorEnergohoneyPrice;
        ValuesHolder.ElevatorAstroluminitePrice             = model.ElevatorAstroluminitePrice;
         
        ValuesHolder.EnergohoneyAsteriumPrice               = model.EnergohoneyAsteriumPrice;
        ValuesHolder.EnergohoneyEnergohoneyPrice            = model.EnergohoneyEnergohoneyPrice;
        ValuesHolder.EnergohoneyAstroluminitePrice          = model.EnergohoneyAstroluminitePrice;
         
        ValuesHolder.AsteriyAsteriumPrice                   = model.AsteriyAsteriumPrice;
        ValuesHolder.AsteriyEnergohoneyPrice                = model.AsteriyEnergohoneyPrice;
        ValuesHolder.AsteriyAstroluminitePrice              = model.AsteriyAstroluminitePrice;
          
        ValuesHolder.CosmodromeAsteriumPrice                = model.CosmodromeAsteriumPrice;
        ValuesHolder.CosmodromeEnergohoneyPrice             = model.CosmodromeEnergohoneyPrice;
        ValuesHolder.CosmodromeAstroluminitePrice           = model.CosmodromeAstroluminitePrice;
          
        ValuesHolder.BedAsteriumPrice                       = model.BedAsteriumPrice;
        ValuesHolder.BedEnergohoneyPrice                    = model.BedEnergohoneyPrice;
        ValuesHolder.BedAstroluminitePrice                  = model.BedAstroluminitePrice;
          
        ValuesHolder.BuildAsteriumPrice                     = model.BuildAsteriumPrice;
        ValuesHolder.BuildEnergohoneyPrice                  = model.BuildEnergohoneyPrice;
        ValuesHolder.BuildAstroluminitePrice                = model.BuildAstroluminitePrice;
          
        ValuesHolder.SupplyAsteriumPrice                    = model.SupplyAsteriumPrice;
        ValuesHolder.SupplyEnergohoneyPrice                 = model.SupplyEnergohoneyPrice;
        ValuesHolder.SupplyAstroluminitePrice               = model.SupplyAstroluminitePrice;
          
        ValuesHolder.ResearchAsteriumPrice                  = model.ResearchAsteriumPrice;
        ValuesHolder.ResearchEnergohoneyPrice               = model.ResearchEnergohoneyPrice;
        ValuesHolder.ResearchAstroluminitePrice             = model.ResearchAstroluminitePrice;
      
        ValuesHolder.RepairCost                             = model.RepairCost;
        ValuesHolder.DamageByTide                           = model.DamageByTide;
        ValuesHolder.DamageByTideMultiplier                 = model.DamageByTideMultiplier;
        ValuesHolder.EnergohoneyConsumeMultiplier           = model.EnergohoneyConsumeMultiplier;
        ValuesHolder.EnergohoneyConsumeMultiplierByRoom     = model.EnergohoneyConsumeMultiplierByRoom;
    }

    public void TryGetBalance()
    {
        path = Application.isEditor ? Application.dataPath + "/Resources" : path = Directory.GetCurrentDirectory();
        if (!File.Exists(path + "/Balance.json")) MakeTemplate(path);
        model = JsonConvert.DeserializeObject<Constants>(File.ReadAllText(path + "/Balance.json"));
    }

    public void MakeTemplate(string path)
    {
        Constants tm = new()
        {
            StartAstroluminite = 6,
            StartAsterium = 40,
            StartEnergohoney = 40,
            InteractionSpeedMultiplyerByLevel = 0.95f,
            InteractionSpeedMultiplyerByGrade = 0.95f,
            MaxTemperature = 20,
            MinTemperature = 25,
            StandartInteractionTime = 18,
            InteracionTimeMultiplyerByCorrectJob = 0.75f,
            DurationLoss = 4,
            RepairSpeed = 3,
            AstroluminiteAmountByOneInteraction = 8,
            EnergohoneyAmountByOneInteraction = 5,
            AsteriumAmountByOneInteraction = 20,
            GameDuration = 480,
            CycleDuration = 120,
            StartPrototype = 0,
            StartUrsowaks = 0,
            PrototypeAmountByOneInteraction = 0,
            UrsowaksAmountByOneInteraction = 0,
            StandartInteractionTimeAsteriumComplex = 0,
            CycleModifier = 1,
            BuyBears = -7,
            BuyHoney = -0.25f,
            BuyTime = -3,
            BuyTemperatureBoost = -7,
            BuyAsterium = -0.12f,
            SellHoney = 0.2f,
            SellAsterium = 0.1f,
            SellAstroluminite = 0.6f,
            SellPrototype = 3,
            SellUrsowaks = 5,

            ElevatorAsteriumPrice = 10,
            ElevatorEnergohoneyPrice = 0,
            ElevatorAstroluminitePrice = 0,

            EnergohoneyAsteriumPrice = 20,
            EnergohoneyEnergohoneyPrice = 25,
            EnergohoneyAstroluminitePrice = 1,

            AsteriyAsteriumPrice = 30,
            AsteriyEnergohoneyPrice = 0,
            AsteriyAstroluminitePrice = 3,

            CosmodromeAsteriumPrice = 0,
            CosmodromeEnergohoneyPrice = 0,
            CosmodromeAstroluminitePrice = 0,

            BedAsteriumPrice = 25,
            BedEnergohoneyPrice = 10,
            BedAstroluminitePrice = 0,

            BuildAsteriumPrice = 35,
            BuildEnergohoneyPrice = 0,
            BuildAstroluminitePrice = 3,

            SupplyAsteriumPrice = 30,
            SupplyEnergohoneyPrice = 5,
            SupplyAstroluminitePrice = 2,

            ResearchAsteriumPrice = 25,
            ResearchEnergohoneyPrice = 0,
            ResearchAstroluminitePrice = 1,

            RepairCost = 10,
            DamageByTide = 7,
            DamageByTideMultiplier = 1,
            EnergohoneyConsumeMultiplier = 1,
            EnergohoneyConsumeMultiplierByRoom = 1,
        }; // tm = templateModel
        //Debug.Log(JsonConvert.SerializeObject(tm, Formatting.Indented));
        File.WriteAllText(path + "/Balance.json", JsonConvert.SerializeObject(tm, Formatting.Indented));
    }
}
