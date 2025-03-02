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
        Debug.Log(model.CycleDuration);

        SetValuesHolder();
    }

    private void SetValuesHolder()
    {
        ValuesHolder.StartAstroluminite                   = model.StartAstroluminite;
        ValuesHolder.StartAsterium                        = model.StartAsterium;
        ValuesHolder.StartEnergohoney                     = model.StartEnergohoney;
        ValuesHolder.InteractionSpeedMultiplyerByLevel    = model.InteractionSpeedMultiplyerByLevel;
        ValuesHolder.InteractionSpeedMultiplyerByGrade    = model.InteractionSpeedMultiplyerByGrade;
        ValuesHolder.MaxTemperature                       = model.MaxTemperature;
        ValuesHolder.MinTemperature                       = model.MinTemperature;
        ValuesHolder.StandartInteractionTime              = model.StandartInteractionTime;
        ValuesHolder.InteracionTimeMultiplyerByCorrectJob = model.InteracionTimeMultiplyerByCorrectJob;
        ValuesHolder.DurationLoss                         = model.DurationLoss;
        ValuesHolder.RepairSpeed                          = model.RepairSpeed;
        ValuesHolder.AstroluminiteAmountByOneInteraction  = model.AstroluminiteAmountByOneInteraction;
        ValuesHolder.EnergohoneyAmountByOneInteraction    = model.EnergohoneyAmountByOneInteraction;
        ValuesHolder.AsteriumAmountByOneInteraction       = model.AsteriumAmountByOneInteraction;
        ValuesHolder.GameDuration                         = model.GameDuration;
        ValuesHolder.CycleDuration                        = model.CycleDuration;
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
            InteractionSpeedMultiplyerByLevel = 1.05f,
            InteractionSpeedMultiplyerByGrade = 0.75f,
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
            CycleDuration = 120
        }; // tm = templateModel
        //Debug.Log(JsonConvert.SerializeObject(tm, Formatting.Indented));
        File.WriteAllText(path + "/Balance.json", JsonConvert.SerializeObject(tm, Formatting.Indented));
    }
}
