using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ModManager : MonoBehaviour
{
	public Constants model;
	private string path = Application.isEditor ? Application.dataPath + "/Resources/Mods" : Directory.GetCurrentDirectory() + "/Mods";
	[SerializeField] private ModQuestManager questManager;
	[SerializeField] private ModEvents eventsManager;
	[Header("Visuals")]
	[SerializeField] private List<SpriteFiller> spriteFillerList;

	public ModEvents GetModEventsManager() => eventsManager;

	public void AddSpriteFiller(SpriteFiller filler)
	{
		spriteFillerList.Add(filler);
	}

	public void ClearSpriteFillers()
	{
		spriteFillerList = new List<SpriteFiller>();
	}

	private void Start()
	{
		Debug.Log(JsonConvert.DeserializeObject<LocalEvent>(JsonConvert.SerializeObject(new LocalEvent
		{
			name = "TestEvent",
			description = "локально игрет",
			start_date_time = "2025-03-27T11:00:00Z",
			duration_in_minutes = 5,
			multipliers = new Dictionary<string, float>() { { "EnergohoneyGain", 2 } }
		})).start_date_time);

		model = MakeTemplate();
		SetValuesHolder();
		TryGetMods();
		//Debug.Log(ValuesHolder.GameDuration);
	}

	private void SetValuesHolder()
	{
		ValuesHolder.StartAstroluminite = model.StartAstroluminite;
		ValuesHolder.StartAsterium = model.StartAsterium;
		ValuesHolder.StartEnergohoney = model.StartEnergohoney;
		ValuesHolder.InteractionSpeedMultiplyerByLevel = model.InteractionSpeedMultiplyerByLevel;
		ValuesHolder.InteractionSpeedMultiplyerByGrade = model.InteractionSpeedMultiplyerByGrade;
		ValuesHolder.MaxTemperature = model.MaxTemperature;
		ValuesHolder.MinTemperature = model.MinTemperature;
		ValuesHolder.StandartInteractionTime = model.StandartInteractionTime;
		ValuesHolder.InteracionTimeMultiplyerByCorrectJob = model.InteracionTimeMultiplyerByCorrectJob;
		ValuesHolder.DurationLoss = model.DurationLoss;
		ValuesHolder.RepairSpeed = model.RepairSpeed;
		ValuesHolder.AstroluminiteAmountByOneInteraction = model.AstroluminiteAmountByOneInteraction;
		ValuesHolder.EnergohoneyAmountByOneInteraction = model.EnergohoneyAmountByOneInteraction;
		ValuesHolder.AsteriumAmountByOneInteraction = model.AsteriumAmountByOneInteraction;
		ValuesHolder.GameDuration = model.GameDuration;
		ValuesHolder.CycleDuration = model.CycleDuration;
		ValuesHolder.StartPrototype = model.StartPrototype;
		ValuesHolder.StartUrsowaks = model.StartUrsowaks;
		ValuesHolder.PrototypeAmountByOneInteraction = model.PrototypeAmountByOneInteraction;
		ValuesHolder.UrsowaksAmountByOneInteraction = model.UrsowaksAmountByOneInteraction;
		ValuesHolder.StandartInteractionTimeAsteriumComplex = model.StandartInteractionTimeAsteriumComplex;
		ValuesHolder.CycleModifier = model.CycleModifier;
		ValuesHolder.BuyBears = model.BuyBears;
		ValuesHolder.BuyHoney = model.BuyHoney;
		ValuesHolder.BuyTime = model.BuyTime;
		ValuesHolder.BuyTemperatureBoost = model.BuyTemperatureBoost;
		ValuesHolder.BuyAsterium = model.BuyAsterium;
		ValuesHolder.SellHoney = model.SellHoney;
		ValuesHolder.SellAsterium = model.SellAsterium;
		ValuesHolder.SellAstroluminite = model.SellAstroluminite;
		ValuesHolder.SellPrototype = model.SellPrototype;
		ValuesHolder.SellUrsowaks = model.SellUrsowaks;

		ValuesHolder.RoomsBuildPrice = model.RoomsBuildPrice;

		ValuesHolder.RepairCost = model.RepairCost;
		ValuesHolder.DamageByTide = model.DamageByTide;
		ValuesHolder.DamageByTideMultiplier = model.DamageByTideMultiplier;
		ValuesHolder.EnergohoneyConsumeMultiplier = model.EnergohoneyConsumeMultiplier;
		ValuesHolder.EnergohoneyConsumeMultiplierByRoom = model.EnergohoneyConsumeMultiplierByRoom;
		ValuesHolder.EnergohoneyConsumeMultiplierByCycle = model.EnergohoneyConsumeMultiplierByCycle;

		ValuesHolder.EnergohoneyExponent = model.EnergohoneyExponent;
	}

	public void TryGetMods()
	{
		//path = Application.isEditor ? Application.dataPath + "/Resources" : path = Directory.GetCurrentDirectory();
		//if (!File.Exists(path + "/config.json")) MakeTemplate(path);
		//model = JsonConvert.DeserializeObject<Constants>(File.ReadAllText(path + "/config.json"));
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
			EventManager.callError.Invoke("Папки Mods не было в файлах игры!");
			return;
		}
		string[] mods = Directory.GetDirectories(path);

		if (mods.Length == 0)
		{
			EventManager.callError.Invoke("Нет модов в папке Mods!");
			return;
		}
		foreach (string mod in mods)
		{
			ParseMod(mod);
		}
		//try
		//{
		//	model = JsonConvert.DeserializeObject<Constants>(File.ReadAllText(path + "/config.json"));
		//}
		//catch
		//{
		//	model = MakeTemplate();
		//	EventManager.callError.Invoke("Не удалось загрузить мод. планету!");
		//}
	}

	/// <summary>
	/// Parses individal mod
	/// </summary>
	/// <param name="directoryPath"></param>
	private void ParseMod(string directoryPath)
	{
		string[] repos = Directory.GetDirectories(directoryPath);

		if (repos.Length == 0)
		{
			EventManager.callError.Invoke($"Мод {directoryPath.Split("\\")[^1]} не содержит в себе контента!");
			return;
		}
		Debug.Log(repos[0]);
		string modDataPath = repos.First(x => x.Split("\\")[^1] == "Data");
		//string modAssetsPath = repos.First(x => x.Split("/")[^1] == "Assets");

		string[] dataRepos = Directory.GetDirectories(modDataPath);
		
		foreach (string dataRepo in dataRepos)
		{
			Debug.Log(dataRepo.Split("\\")[^1]);
			switch (dataRepo.Split("\\")[^1])
			{
				case "Quests":
					ParseQuests(dataRepo);
					break;
				case "Events":
					ParseEvents(dataRepo);
					break;
				default:
					EventManager.callError.Invoke($"Неизвестная директория {dataRepo}!");
					break;
			}
		}

		//string[] assetRepos = Directory.GetDirectories(modDataPath);
		//foreach (string assetRepo in assetRepos)
		//{
		//	switch (assetRepo.Split("/")[^1])
		//	{
		//		case "CoreVisuals":
		//			ParseCoreVisuals(assetRepo);
		//			break;
		//		case "Events":

		//			break;
		//		default:
		//			EventManager.callError.Invoke($"Неизвестная директория {assetRepo}!");
		//			break;
		//	}
		//}
	}

	private void ParseCoreVisuals(string assetRepo)
	{
		string[] files = Directory.GetFiles(assetRepo);
		foreach (string file in files)
		{
			string fileName = file.Split("\\")[^1];
			Debug.Log(fileName);
			spriteFillerList.FirstOrDefault(x => x._name == fileName);
			//Texture2D texture = new Texture2D(2, 2);
			//texture.LoadImage(File.ReadAllBytes(file));

		}
	}

	private void ParseEvents(string eventsPath)
	{
		string[] events = Directory.GetFiles(eventsPath);
		Debug.Log($"<color=red>{events[0]} and {events[1]}");
		if (events.Length == 0)
		{
			Debug.Log("<color=red>СОСО");
			EventManager.callError.Invoke("Папка Events пуста!");
			return;
		}
		eventsManager.ResetTicker();
		foreach (string localEvent in events)
		{
			if (localEvent.Contains(".meta")) continue;
			string rawEventContent = File.ReadAllText(localEvent);
			LocalEvent newEvent = null;
			Debug.Log(rawEventContent); 
			try { newEvent = JsonConvert.DeserializeObject<LocalEvent>(rawEventContent); Debug.Log($"<color=yellow>{newEvent.start_date_time}"); }
			catch (Exception e) { EventManager.callError.Invoke($"Файл {localEvent.Split("\\")[^1]} содержит ошибки!"); continue; }
			Debug.Log(newEvent.name);
			eventsManager.localEvents.Add(newEvent);
		}
		eventsManager.StartTicker();
	}

	private void ParseQuests(string questsPath)
	{
		string[] quests = Directory.GetFiles(questsPath);
		if (quests.Length == 0)
		{
			EventManager.callError.Invoke("Папка Quests пуста!");
			return;
		}
		foreach (string quest in quests)
		{
			string rawQuestContent = File.ReadAllText(quest);
			Quest newQuest = null;
			try { newQuest = JsonConvert.DeserializeObject<Quest>(rawQuestContent); }
			catch (Exception e) { EventManager.callError.Invoke($"Файл {quest.Split("/")[^1]} содержит ошибки!"); continue; }
			questManager.AddCondition(newQuest);
		}
	}

	public Constants MakeTemplate()
	{
		Constants tm = new()
		{
			StartAstroluminite = 6,
			StartAsterium = 20,
			StartEnergohoney = 40.0f,
			InteractionSpeedMultiplyerByLevel = 0.85f,
			InteractionSpeedMultiplyerByGrade = 0.85f,
			MaxTemperature = 20.0f,
			MinTemperature = 25.0f,
			StandartInteractionTime = 23.0f,
			InteracionTimeMultiplyerByCorrectJob = 0.75f,
			DurationLoss = 4.0f,
			RepairSpeed = 3.0f,
			AstroluminiteAmountByOneInteraction = 4,
			EnergohoneyAmountByOneInteraction = 9.0f,
			AsteriumAmountByOneInteraction = 30,
			GameDuration = 960.0f,
			CycleDuration = 240.0f,
			StartPrototype = 0,
			StartUrsowaks = 0,
			PrototypeAmountByOneInteraction = 1,
			UrsowaksAmountByOneInteraction = 1,
			StandartInteractionTimeAsteriumComplex = 40f,
			CycleModifier = 1.0f,
			BuyBears = -7.0f,
			BuyHoney = -0.27f,
			BuyTime = -3.0f,
			BuyTemperatureBoost = -7.0f,
			BuyAsterium = -0.17f,
			SellHoney = 0.2f,
			SellAsterium = 0.1f,
			SellAstroluminite = 0.6f,
			SellPrototype = 3.0f,
			SellUrsowaks = 5.0f,

			RoomsBuildPrice = new()
			{
				{ RoomType.Elevator,    new(){ { ResourceType.AsteriumPrice, 10 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 0 } } },
				{ RoomType.Energohoney, new(){ { ResourceType.AsteriumPrice, 20 }, { ResourceType.EnergohoneyPrice, 25 }, { ResourceType.AstroluminitePrice, 1 } } },
				{ RoomType.Asterium,    new(){ { ResourceType.AsteriumPrice, 30 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 3 } } },
				{ RoomType.Cosmodrome,  new(){ { ResourceType.AsteriumPrice, 0 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 0 } } },
				{ RoomType.Bed,         new(){ { ResourceType.AsteriumPrice, 25 }, { ResourceType.EnergohoneyPrice, 10 }, { ResourceType.AstroluminitePrice, 0 } } },
				{ RoomType.Build,       new(){ { ResourceType.AsteriumPrice, 35 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 3 } } },
				{ RoomType.Supply,      new(){ { ResourceType.AsteriumPrice, 30 }, { ResourceType.EnergohoneyPrice, 5 }, { ResourceType.AstroluminitePrice, 2 } } },
				{ RoomType.Research,    new(){ { ResourceType.AsteriumPrice, 25 }, { ResourceType.EnergohoneyPrice, 0 }, { ResourceType.AstroluminitePrice, 1 } } }
			},

			RepairCost = 15,
			DamageByTide = 7.0f,
			DamageByTideMultiplier = 1.0f,
			EnergohoneyConsumeMultiplier = 1.0f,
			EnergohoneyConsumeMultiplierByRoom = 1.0f,
			EnergohoneyConsumeMultiplierByCycle = 5.2f,
			EnergohoneyExponent = 1f,
		};
		//Debug.Log(JsonConvert.SerializeObject(tm, Formatting.Indented));
		// File.WriteAllText(path + "/config.json", JsonConvert.SerializeObject(tm, Formatting.Indented));
		return tm;
	}
}

