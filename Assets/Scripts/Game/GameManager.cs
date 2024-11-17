using System.Collections.Generic;
using UnityEngine;
using API.Sevices.Mapper;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public static RequestManager RequestManager = new RequestManager();
	public Tilemap tilemap;
	public List<GameObject> bears = new List<GameObject>();
	[SerializeField] private GameObject selectedUnit;
	[SerializeField] private GameObject buildingScreen;
	private GameObject queuedBuildPositon;

	private int honey;
	private int asteriy;

	RaycastHit raycastHit;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void QueueBuildPos(GameObject queuePos)
	{
		queuedBuildPositon = queuePos;
		buildingScreen.SetActive(true);
	}

	public void SelectAndBuild(GameObject building)
	{
		var instance = Instantiate(building, queuedBuildPositon.transform.position, Quaternion.identity);
		if (instance.TryGetComponent<RoomScript>(out var output))
		{
			output.connectedElevators = queuedBuildPositon.GetComponentInParent<RoomScript>().connectedElevators;
			if (tilemap.HasTile(tilemap.WorldToCell(queuedBuildPositon.transform.parent.position) - new Vector3Int(2, 0, 0)))
			{
				var tile = tilemap.GetTile(tilemap.WorldToCell(queuedBuildPositon.transform.parent.position) - new Vector3Int(2, 0, 0));
				if (tile.name.Contains("Elevator"))
				{
					var elevator = tile.GetComponentInParent<Elevator>();
					elevator.connectedRooms.Add(output);
					elevator.connectedElevators.Add(elevator.connectedElevators.Contains(output.connectedElevators[0]) ? (elevator.connectedElevators.Contains(output.connectedElevators[1]) ? null : output.connectedElevators[1]) : output.connectedElevators[0]);
				}
				else
				{
					var room = tilemap.GetTile(tilemap.WorldToCell(queuedBuildPositon.transform.parent.position) - new Vector3Int(2, 0, 0)).GetComponent<RoomScript>();
					room.connectedElevators = output.connectedElevators;
				}
			}
			else if (tilemap.HasTile(tilemap.WorldToCell(queuedBuildPositon.transform.parent.position) + new Vector3Int(1, 0, 0)))
			{
				var tile = tilemap.GetTile(tilemap.WorldToCell(queuedBuildPositon.transform.parent.position) + new Vector3Int(1, 0, 0));
				if (tile.name.Contains("Elevator"))
				{
					var elevator = tile.GetComponentInParent<Elevator>();
					elevator.connectedRooms.Add(output);
					elevator.connectedElevators.Add(elevator.connectedElevators.Contains(output.connectedElevators[0]) ? (elevator.connectedElevators.Contains(output.connectedElevators[1]) ? null : output.connectedElevators[1]) : output.connectedElevators[0]);
				}
				else
				{
					var room = tilemap.GetTile(tilemap.WorldToCell(queuedBuildPositon.transform.parent.position) + new Vector3Int(1, 0, 0)).GetComponent<RoomScript>();
					room.connectedElevators = output.connectedElevators;
				}
			}
		}
		else if (instance.TryGetComponent<Elevator>(out var elevatorOutput))
		{

			elevatorOutput.connectedRooms.Add(queuedBuildPositon.GetComponentInChildren<RoomScript>());
		}
	}

	/// <summary>
	/// Returns amount of honey on current client
	/// </summary>
	/// <returns></returns>
	public int GetHoney()
	{
		return honey;
	}

	/// <summary>
	/// Returns amount of Asteriun on current client
	/// </summary>
	/// <returns></returns>
	public int GetAsteriy()
	{
		return asteriy;
	}

	/// <summary>
	/// Changes amount of honey by given number
	/// </summary>
	/// <param name="amount"></param>
	public void ChangeHoney(int amount)
	{
		honey += amount;
	}

	/// <summary>
	/// Changes amount of asterium by given number
	/// </summary>
	/// <param name="amount"></param>
	public void ChangeAsteriy(int amount)
	{
		asteriy += amount;
	}

	private void Update()
	{
		InputHandler();
	}

	private void InputHandler()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out raycastHit, 100f))
			{
				if (raycastHit.transform != null)
				{
					ClickedGameObject(raycastHit.transform.gameObject);
				}
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out raycastHit, 100f))
			{
				if (raycastHit.transform != null)
				{
					RightClick(raycastHit.transform.gameObject);
				}
			}
		}
	}

	private void RightClick(GameObject gameObject)
	{
		if (gameObject.CompareTag("room"))
		{
			selectedUnit.GetComponent<UnitMovement>().StopAllCoroutines();
			selectedUnit.GetComponent<UnitMovement>().MoveToRoom(gameObject.GetComponent<RoomScript>());
		}
	}

	private void ClickedGameObject(GameObject gameObject)
	{
		if (gameObject.CompareTag("unit"))
		{
			//gameObject.GetComponent<UnitScript>().ChooseUnit();
			selectedUnit = gameObject;
		}
		else
		{
			selectedUnit = null;
		}
	}
}
