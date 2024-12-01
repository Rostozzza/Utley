using Newtonsoft.Json;
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
		int honey = await GameManager.Instance.GetHoney();
		int asterium = await GameManager.Instance.GetAsteriy();
		playerModel.resources.Add("honey", honey.ToString());
		playerModel.resources.Add("asterium", asterium.ToString());
		playerModel.resources.Add("password", GameManager.Instance.playerModel.resources["password"]);

		string serializedPlayer = JsonConvert.SerializeObject(playerModel);
		//File.WriteAllText(path,serializedPlayer);
		var allPlayers = await requestManager.GetAllPlayers();
		if (allPlayers.FirstOrDefault(x => x.name == playerName) != null)
		{
			Debug.Log(JsonConvert.SerializeObject(playerModel));
			var rerponce = await requestManager.UpdatePlayerResources(playerName,playerModel.resources);
		}
		return playerModel;
	}

	public async Task<Player> CreateNewPlayer(string playerName,string password)
	{
		Player playerModel = new Player();
		playerModel.name = playerName;
		playerModel.resources = new Dictionary<string, string>();
		//var bearsToSave = JsonConvert.SerializeObject(GameManager.Instance.bears.Select(x => x.GetComponent<UnitScript>().bearModel).ToList());
		//playerModel.Resources.Add("bears", bearsToSave);
		string path = $"{Application.persistentDataPath} + /{playerName}.json";

		//playerModel.Resources.Add("rooms", SerializeRooms());
		//playerModel.Resources.Add("elevators", SerializeElevators());

		playerModel.resources.Add("honey", "100");
		playerModel.resources.Add("asterium", "100");
		playerModel.resources.Add("password",password);

		string serializedPlayer = JsonConvert.SerializeObject(playerModel);
		//File.WriteAllText(path, serializedPlayer);
		if (requestManager.GetAllPlayers().Result.FirstOrDefault(x => x.name == playerName) != null)
		{
			Debug.Log("Player already exists!");
			return null;
		}
		var rerponce = await requestManager.CreatePlayer(playerModel);
		return rerponce;
	}

	public void CreateLog(Log log)
	{
		string path = $"{Application.persistentDataPath} + /{log.Comment}.json";
		requestManager.CreateLog(log).Wait();
	}

	public Shop InitializeShop()
	{
		Shop shop = new Shop
		{
			Name = "SpaceshipShop",
			Resources = new Dictionary<string, int>()
		};
		shop.Resources.Add("asterium", 300);
		shop.Resources.Add("bears", 100);
		return shop;
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
		if (bearsToAdd != null)
		{
			foreach (var bear in bearsToAdd)
			{
				GameManager.Instance.LoadBearFromModel(bear);
			}
		}*/
		try
		{
			var rooms = JsonConvert.DeserializeObject<List<Room>>(model.resources["rooms"]);
			var elevators = JsonConvert.DeserializeObject<List<ElevatorModel>>(model.resources["elevators"]);
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
		}
		catch { Debug.Log("no rooms to spawn!"); }
	}
}