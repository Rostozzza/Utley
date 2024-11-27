using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public class JsonManager
{
	RequestManager requestManager = new RequestManager();

	public string SavePlayerToJson(string playerName)
	{
		Player playerModel = new Player();
		playerModel.Name = playerName;
		playerModel.Resources = new Dictionary<string, string>();
		//var bearsToSave = JsonConvert.SerializeObject(GameManager.Instance.bears.Select(x => x.GetComponent<UnitScript>().bearModel).ToList());
		//playerModel.Resources.Add("bears", bearsToSave);
		string path = $"Assets/Resources/Players/{playerName}.json";
		playerModel.Resources.Add("rooms", SerializeRooms());
		playerModel.Resources.Add("elevators", SerializeElevators());
		return JsonConvert.SerializeObject(playerModel);
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
		//var bearsToAdd = JsonConvert.DeserializeObject<List<Bear>>(model.Resources["bears"]);
		//if (bearsToAdd != null)
		//{
		//	foreach (var bear in bearsToAdd)
		//	{
		//		GameManager.Instance.LoadBearFromModel(bear);
		//	}
		//}
		var rooms = JsonConvert.DeserializeObject<List<Room>>(model.Resources["rooms"]);
		var elevators = JsonConvert.DeserializeObject<List<ElevatorModel>>(model.Resources["elevators"]);
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
}