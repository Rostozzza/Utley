using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public class JsonManager
{
	RequestManager requestManager = new RequestManager();

	public void SavePlayerToJson(string playerName)
	{
		Player playerModel = new Player();
		playerModel.Name = playerName;
		var bearsToSave = JsonConvert.SerializeObject(GameManager.Instance.bears.Select(x => x.GetComponent<UnitScript>().bearModel).ToList());
		playerModel.Resources.Add("bears", bearsToSave);
		string path = $"Assets/Resources/Players/{playerName}.json";
		playerModel.Resources.Add("rooms", SerializeRooms());
		playerModel.Resources.Add("elevators", SerializeElevators());
	}

	public string SerializeRooms()
	{
		List<Room> rooms = new List<Room>();
		foreach (var room in GameManager.Instance.allRooms.Where(x => x.GetComponent<RoomScript>()).Select(y => y.GetComponent<RoomScript>()).ToList())
		{
			Room roomModel = new Room();
			roomModel.Coordinates = new List<float> { room.transform.position.x, room.transform.position.y, room.transform.position.z };
			roomModel.Index = GameManager.Instance.allRooms.IndexOf(room.gameObject);
			roomModel.ConnectedElevators = GameManager.Instance.allRooms.Where(x => x.GetComponent<Elevator>()).Select(y => GameManager.Instance.allRooms.IndexOf(y)).ToList();
			roomModel.Level = room.level;
			roomModel.Type = room.resource.ToString();
			rooms.Add(roomModel);
		}
		return JsonConvert.SerializeObject(rooms);
	}

	public string SerializeElevators()
	{
		List<ElevatorModel> elevators = new List<ElevatorModel>();
		foreach (var elevator in GameManager.Instance.allRooms.Where(x => x.GetComponent<Elevator>()).Select(y => y.GetComponent<Elevator>()).ToList())
		{
			ElevatorModel elevatorModel = new ElevatorModel();
			elevatorModel.ConnectedRooms = GameManager.Instance.allRooms.Where(x => elevator.connectedRooms.Contains(x.GetComponent<RoomScript>())).Select(y => GameManager.Instance.allRooms.IndexOf(y)).ToList();
			elevatorModel.ConnectedElevators = GameManager.Instance.allRooms.Where(x => x.GetComponent<Elevator>()).Select(y => GameManager.Instance.allRooms.IndexOf(y)).ToList();
			elevatorModel.Coordinates = new List<float> { elevator.gameObject.transform.position.x, elevator.gameObject.transform.position.y, elevator.gameObject.transform.position.z };
			elevators.Add(elevatorModel);
		}
		return JsonConvert.SerializeObject(elevators);
	}

	public void LoadPlayerFromModel(Player model)
	{
		var bearsToAdd = JsonConvert.DeserializeObject<List<Bear>>(model.Resources["bears"]);
		GameManager.Instance.playerModel = model;
		if (bearsToAdd != null)
		{
			foreach (var bear in bearsToAdd)
			{
				GameManager.Instance.LoadBearFromModel(bear);
			}
		}
		var rooms = JsonConvert.DeserializeObject<List<Room>>(model.Resources["rooms"]);
		var elevators = JsonConvert.DeserializeObject<List<ElevatorModel>>(model.Resources["elevators"]);
		for (int i = 0; i < rooms.Count + elevators.Count; i++)
		{
			GameManager.Instance.LoadRoomFromModel(rooms[i]);
		}
	}
}