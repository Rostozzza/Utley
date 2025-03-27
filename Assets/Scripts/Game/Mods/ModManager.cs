using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using static TutorialManager;

public class ModManager : MonoBehaviour
{
	public Constants model;
	private string path = Application.isEditor ? Application.dataPath + "/Resources/Mods" : Directory.GetCurrentDirectory() + "/Mods";
	[SerializeField] private ModQuestManager questManager;
	[SerializeField] private ModEvents eventsManager;
	[Header("Visuals")]
	[SerializeField] public List<Sprite> earthSprites = new List<Sprite>();

	public ModEvents GetModEventsManager() => eventsManager;


	private void Start()
	{
		path = Application.isEditor ? Application.dataPath + "/Resources/Mods" : path = Directory.GetCurrentDirectory() + "/Mods";
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		if (!File.Exists(path + "/config.json")) MakeTemplate();
		model = JsonConvert.DeserializeObject<Constants>(File.ReadAllText(path + "/config.json"));
		SetValuesHolder();
		//TryGetMods();
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

	private void ParseCondigSprites()
	{
		var files = Directory.GetFiles(path+"/Assets/Config");
		if (files.Length == 0) return;
		foreach (var file in files)
		{
			Texture2D SpriteTexture = new Texture2D(2, 2);
			SpriteTexture.LoadImage(File.ReadAllBytes(file));
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, 0, 0), new Vector2(0, 0));
			earthSprites.Add(NewSprite);
		}
	}

	public void TryGetMods()
	{
		path = Application.isEditor ? Application.dataPath + "\\Resources\\Mods" : path = Directory.GetCurrentDirectory() + "/Mods";
		//if (!Directory.Exists(path+"/Data")) Directory.CreateDirectory(path + "\\Data");
		if (!File.Exists(path + "\\Data/config.json")) MakeTemplate();
		model = JsonConvert.DeserializeObject<Constants>(File.ReadAllText(path + "\\Data\\config.json"));
		ParseCondigSprites();
		SetValuesHolder();
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
			EventManager.callError.Invoke("Папки Mods не было в файлах игры!");
			return;
		}
		ParseMod();
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
	private void ParseMod()
	{
		string[] dataRepos = Directory.GetFiles(path + "\\Data");
		string[] assetsRepos = Directory.GetDirectories(path + "\\Assets");

		foreach (string dataRepo in dataRepos)
		{
			Debug.Log(dataRepo.Split("\\")[^1]);
			if (dataRepo.Contains(".meta")) continue;
			switch (dataRepo.Split("\\")[^1])
			{
				case "Quests.json":
					ParseQuests(dataRepo);
					break;
				case "Events":
					ParseEvents(dataRepo);
					break;
				case "Config":
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
			//spriteFillerList.FirstOrDefault(x => x._name == fileName);
			Texture2D texture = new Texture2D(256, 256);
			texture.LoadImage(File.ReadAllBytes(file));
			Texture2D SpriteTexture = new Texture2D(2, 2);
			SpriteTexture.LoadImage(File.ReadAllBytes(file));
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0));
		}
	}


	private void ParseEvents(string eventsPath)
	{
		eventsManager.ResetTicker();
		string rawQuestContent = File.ReadAllText(eventsPath);
		List<LocalEvent> events = null;
		try { events = JsonConvert.DeserializeObject<List<LocalEvent>>(rawQuestContent); }
		catch (Exception e) { EventManager.callError.Invoke($"Файл Events содержит ошибки!"); }
		if (events != null)
		{
			foreach (var quest in events)
			{
				Debug.Log($"<color=green>{quest.name}");
				quest.icon_path = path + "\\Assets\\" + quest.icon_path;
				eventsManager.localEvents.Add(quest);
			}
		}
		eventsManager.StartTicker();
	}

	private void ParseQuests(string questsPath)
	{
		Debug.Log("<color=red>PARSING QUESTS");
		string rawQuestContent = File.ReadAllText(questsPath);
		List<Quest> quests = null;
		//Debug.Log(JsonConvert.SerializeObject(new List<Quest> {new Quest { name="Test",description="test",condition=new KeyValuePair<string, int>("SurviveFor",920)} }));
		try { quests = JsonConvert.DeserializeObject<List<Quest>>(rawQuestContent); }
		catch (Exception e) { EventManager.callError.Invoke($"Файл Quests содержит ошибки!"); }
		if (quests != null)
		{
            foreach (var quest in quests)
			{
				Debug.Log($"<color=green>{quest.name}");
				quest.icon_path = path+"\\Assets\\"+quest.icon_path;
				questManager.AddCondition(quest);
			}
		}
	}

	public void MakeTemplate()
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
		File.WriteAllText(path + "/config.json", JsonConvert.SerializeObject(tm, Formatting.Indented));
	}
}

