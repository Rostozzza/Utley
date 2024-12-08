using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class JsonManager
{
	RequestManager requestManager;
	public bool isAPIActive;

	public JsonManager(bool isAPIActive)
	{
		this.isAPIActive = isAPIActive;
		requestManager = new RequestManager(isAPIActive);
	}

	public async Task<Player> SavePlayerToJson(string playerName)
	{
		Player playerModel = new Player();
		playerModel.name = playerName;
		playerModel.resources = new Dictionary<string, string>();
		var bearsToSave = JsonConvert.SerializeObject(GameManager.Instance.bears.Select(x => x.GetComponent<UnitScript>().bearModel).ToList());
		//playerModel.resources.Add("bears", bearsToSave);
		string path = $"{Application.persistentDataPath} + /{playerName}.json";
		playerModel.resources.Add("rooms", SerializeRooms().ToString());
		playerModel.resources.Add("elevators", SerializeElevators().ToString());
		float honey = GameManager.Instance.honey;
		int asterium = GameManager.Instance.asteriy;
		float ursowaks = GameManager.Instance.ursowaks;
		float astroluminite = GameManager.Instance.astroluminite;
		float prototype = GameManager.Instance.prototype;
		float HNY = GameManager.Instance.HNY;
		playerModel.resources.Add("honey", honey.ToString());
		playerModel.resources.Add("asterium", asterium.ToString());
		playerModel.resources.Add("ursowaks", ursowaks.ToString());
		playerModel.resources.Add("astroluminite", astroluminite.ToString());
		playerModel.resources.Add("prototype", prototype.ToString());
		playerModel.resources.Add("bears", GameManager.Instance.playerBears.ToString());
		playerModel.resources.Add("HNY", HNY.ToString());
		playerModel.resources.Add("password", GameManager.Instance.playerModel.resources["password"]);

		string serializedPlayer = JsonConvert.SerializeObject(playerModel);
		//File.WriteAllText(path,serializedPlayer);
		//var allPlayers = await requestManager.GetAllPlayers();
		//if (allPlayers.FirstOrDefault(x => x.name == playerName) != null)
		//{
		await requestManager.UpdatePlayerResources(playerName, playerModel.resources);
		//}
		return playerModel;
	}

	public async Task<Player> CreateNewPlayer(string playerName, string password)
	{
		Player playerModel = new Player();
		playerModel.name = playerName;
		playerModel.resources = new Dictionary<string, string>();
		//var bearsToSave = JsonConvert.SerializeObject(GameManager.Instance.bears.Select(x => x.GetComponent<UnitScript>().bearModel).ToList());
		//playerModel.Resources.Add("bears", bearsToSave);
		string path = $"{Application.persistentDataPath} + /{playerName}.json";

		playerModel.resources.Add("rooms", "[{\"Type\":\"AsteriumRecycle\",\"Level\":1,\"Coordinates\":[-2.8,10.0,0.0],\"ConnectedElevators\":[1],\"Durability\":1,\"Index\":0},{\"Type\":\"BuildRoom\",\"Level\":1,\"Coordinates\":[-2.8,2.0,0.0],\"ConnectedElevators\":[1],\"Durability\":1,\"Index\":2},{\"Type\":\"EnergoHoneyRoom\",\"Level\":1,\"Coordinates\":[-10.8,10.0,0.0],\"ConnectedElevators\":[1],\"Durability\":1,\"Index\":3},{\"Type\":\"ResearchRoom\",\"Level\":1,\"Coordinates\":[-18.8,10.0,0.0],\"ConnectedElevators\":[1],\"Durability\":1,\"Index\":4},{\"Type\":\"SupplyRoom\",\"Level\":1,\"Coordinates\":[-2.8,6.0,0.0],\"ConnectedElevators\":[1],\"Durability\":1,\"Index\":5},{\"Type\":\"LivingRoom\",\"Level\":1,\"Coordinates\":[-10.8,6.0,0.0],\"ConnectedElevators\":[1],\"Durability\":1,\"Index\":6},{\"Type\":\"complex_cosmodrome (1)\",\"Level\":1,\"Coordinates\":[-5.8,14.0,0.0],\"ConnectedElevators\":[1],\"Durability\":1,\"Index\":7}]");
		playerModel.resources.Add("elevators", "[{\"Coordinates\":[5.2,10.0,0.0],\"ConnectedElevators\":[1],\"ConnectedRooms\":[0,2,5,7],\"BlocksUp\":1,\"BlocksDown\":2,\"Index\":1}]");

		playerModel.resources.Add("honey", "100");
		playerModel.resources.Add("asterium", "100");
		playerModel.resources.Add("ursowaks", "0");
		playerModel.resources.Add("astroluminite", "0");
		playerModel.resources.Add("prototype", "0");
		playerModel.resources.Add("HNY", "0");
		playerModel.resources.Add("bears", "4");
		playerModel.resources.Add("password", password);
		string serializedPlayer = JsonConvert.SerializeObject(playerModel);
		//File.WriteAllText(path, serializedPlayer);
		var rerponce = await requestManager.CreatePlayer(playerModel);
		return rerponce;
	}

	public async Task CreateLog(Log log)
	{
		//string path = $"{Application.persistentDataPath} + /{log.Comment}.json";
		await requestManager.CreateLog(log);
	}

	public async Task<Shop> InitializeShop(string player)
	{
		Shop shop = new Shop
		{
			name = "HNYShop",
			resources = new Dictionary<string, int>()
		};
		shop.resources.Add("bears", 2);
		shop.resources.Add("time", 1);
		shop.resources.Add("honey", 300);
		shop.resources.Add("temperatureBoost", 1);
		shop.resources.Add("asterium",500);
		if (isAPIActive)
		{
			await requestManager.CreateShop(player, shop);
		}
		return shop;
	}

	public async Task<Shop> SaveShopToJson(string shopName)
	{
		Shop shopModel = new Shop();
		shopModel.name = shopName;
		shopModel.resources = new Dictionary<string, int>();
		ShopManager shopController = ShopManager.Instance;
		var shopHoney = shopController.honey;
		var shopBears = shopController.bears;
		var shopTime = shopController.time;
		var shopTemperatureBoost = shopController.temperatureBoost;
		var shopAsterium = shopController.asterium;

		shopModel.resources.Add("honey", shopHoney);
		shopModel.resources.Add("bears", shopBears);
		shopModel.resources.Add("time", shopTime);
		shopModel.resources.Add("temperatureBoost", shopTemperatureBoost);
		shopModel.resources.Add("asterium", shopAsterium);

		string serializedShop = JsonConvert.SerializeObject(shopModel);
		Debug.Log(serializedShop);
		//File.WriteAllText(path,serializedPlayer);
		//var allPlayers = await requestManager.GetAllPlayers();
		//if (allPlayers.FirstOrDefault(x => x.name == playerName) != null)
		//{
		await requestManager.UpdateShopResources(GameManager.Instance.playerName, shopModel.name, shopModel.resources);
		//}
		return shopModel;
	}

	public async Task<Shop> GetShopModel(string shopName)
	{
		Shop requestedShop = await requestManager.GetPlayerShop(GameManager.Instance.playerName, shopName);
		ShopManager.Instance.model = requestedShop;
		return requestedShop;
	}

	private string SerializeRooms()
	{
		List<Room> rooms = new List<Room>();
		foreach (var room in GameManager.Instance.allRooms.Where(x => x.GetComponent<RoomScript>()).Select(y => y.GetComponent<RoomScript>()).ToList())
		{
			Room roomModel = new Room
			{
				Coordinates = new List<float> { room.transform.position.x, room.transform.position.y, room.transform.position.z },
				Index = GameManager.Instance.allRooms.IndexOf(room.gameObject),
				ConnectedElevators = GameManager.Instance.allRooms.Where(x => x.GetComponent<Elevator>()).Select(y => GameManager.Instance.allRooms.IndexOf(y)).ToList(),
				Durability = room.durability,
				Level = room.level,
				Type = room.gameObject.name
			};
			rooms.Add(roomModel);
		}
		Debug.Log(JsonConvert.SerializeObject(rooms, Formatting.Indented));
		return JsonConvert.SerializeObject(rooms);
	}

	private string SerializeElevators()
	{
		List<ElevatorModel> elevators = new List<ElevatorModel>();
		foreach (var elevator in GameManager.Instance.allRooms.Where(x => x.GetComponent<Elevator>()).Select(y => y.GetComponent<Elevator>()).ToList())
		{
			ElevatorModel elevatorModel = new ElevatorModel();
			elevatorModel.ConnectedRooms = GameManager.Instance.allRooms.Where(x => elevator.connectedRooms.Contains(x.GetComponent<RoomScript>())).Select(y => GameManager.Instance.allRooms.IndexOf(y)).ToList();
			elevatorModel.BlocksUp = elevator.transform.GetComponentsInChildren<BuildRoomScript>().Where(x => x.transform.position.y > elevator.transform.position.y && x.gameObject != elevator.gameObject).ToList().Count;
			elevatorModel.BlocksDown = elevator.transform.GetComponentsInChildren<BuildRoomScript>().Where(x => x.transform.position.y < elevator.transform.position.y && x.gameObject != elevator.gameObject).ToList().Count;
			elevatorModel.ConnectedElevators = GameManager.Instance.allRooms.Where(x => x.GetComponent<Elevator>()).Select(y => GameManager.Instance.allRooms.IndexOf(y)).ToList();
			elevatorModel.Coordinates = new List<float> { elevator.gameObject.transform.position.x, elevator.gameObject.transform.position.y, elevator.gameObject.transform.position.z };
			elevatorModel.Index = GameManager.Instance.allRooms.IndexOf(elevator.gameObject);
			elevators.Add(elevatorModel);
		}
		return JsonConvert.SerializeObject(elevators);
	}

	public void LoadPlayerFromModel(Player model)
	{
		GameManager.Instance.playerModel = model;
		/*var bearsToAdd = JsonConvert.DeserializeObject<List<Bear>>(model.Resources["bears"]);
		 GameManager.Instance.KillAllBears();
		if (bearsToAdd != null)
		{
			foreach (var bear in bearsToAdd)
			{
				GameManager.Instance.LoadBearFromModel(bear);
			}
		}*/

		GameManager.Instance.KillAllBuildings();
		//try
		//{
		var rooms = JsonConvert.DeserializeObject<List<Room>>(model.resources["rooms"]);
		var elevators = JsonConvert.DeserializeObject<List<ElevatorModel>>(model.resources["elevators"]);
		Debug.Log($"rooms: {rooms.Count}");
		Debug.Log($"elevators: {elevators.Count}");
		for (int i = 0; i < rooms.Count + elevators.Count; i++)
		{
			try
			{
				if (GameManager.Instance.LoadRoomFromModel(rooms.First(x => x.Index == i)))
				{
					continue;
				}
				else
				{
					GameManager.Instance.LoadElevatorFromModel(elevators.First(x => x.Index == i));
					continue;
				}
			}
			catch
			{
				GameManager.Instance.LoadElevatorFromModel(elevators.First(x => x.Index == i));
			}
		}
		GameManager.Instance.AssembleBase();
		GameManager.Instance.LoadAllBears();
		GameManager.Instance.EnpowerAllRooms();
		//}
		//catch (Exception e) { Debug.Log($"Exception dropped! {e.Message}"); }
	}
}