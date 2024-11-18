using System.Collections.Generic;
using UnityEngine;
using API.Sevices.Mapper;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public static RequestManager RequestManager = new RequestManager();
	public Tilemap tilemap;
	public List<GameObject> bears = new List<GameObject>();
	[SerializeField] private GameObject selectedUnit;
	[SerializeField] private GameObject buildingScreen;
	[SerializeField] private GameObject floorPrefab;
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

	/// <summary>
	/// Saves position at "queuedBuildPosition"
	/// </summary>
	/// <param name="queuePos"></param>
	public void QueueBuildPos(GameObject queuePos)
	{
		queuedBuildPositon = queuePos;
		buildingScreen.SetActive(true);
	}

	/// <summary>
	/// Builds a room from variable "building" at position of "queuedBuildPosition"
	/// </summary>
	/// <param name="building"></param>
	public void SelectAndBuild(GameObject building)
	{
		var instance = Instantiate(building, queuedBuildPositon.transform.position, Quaternion.identity);
		if (instance.CompareTag("elevator"))// trying to build elevator
		{
			RaycastHit hit;
			Ray rayLeft = new Ray(instance.transform.position,Vector3.left);
			var elevator = instance.GetComponent<Elevator>();
			if (queuedBuildPositon.transform.parent.parent.CompareTag("elevator"))
			{
				elevator = queuedBuildPositon.transform.parent.parent.GetComponent<Elevator>();
			}
			if (Physics.Raycast(rayLeft, out hit, 6f))
			{
				if (hit.collider.CompareTag("room"))
				{
					foreach (var e in hit.transform.GetComponent<RoomScript>().connectedElevators)
					{
						e.connectedElevators.Add(elevator);
						instance.GetComponent<Elevator>().connectedElevators.Add(e);
						foreach (var r in e.connectedRooms)
						{
							if (r.transform.position.y == hit.transform.position.y)
							{
								r.connectedElevators.Add(elevator);
							}
						}
					}
					hit.transform.GetComponent<RoomScript>().connectedElevators.Add(elevator);
					elevator.connectedRooms.Add(hit.transform.GetComponent<RoomScript>());
					elevator.connectedElevators.Add(elevator);
				}
			}
			Ray rayRight = new Ray(instance.transform.position, Vector3.right);
			if (Physics.Raycast(rayLeft, out hit, 12f))
			{
				if (hit.collider.CompareTag("room"))
				{
					foreach (var e in hit.transform.GetComponent<RoomScript>().connectedElevators)
					{
						e.connectedElevators.Add(elevator);
						instance.GetComponent<Elevator>().connectedElevators.Add(e);
						foreach (var r in e.connectedRooms)
						{
							if (r.transform.position.y == hit.transform.position.y)
							{
								r.connectedElevators.Add(elevator);
							}
						}
					}
					hit.transform.GetComponent<RoomScript>().connectedElevators.Add(elevator);
					elevator.connectedRooms.Add(hit.transform.GetComponent<RoomScript>());
					elevator.connectedElevators.Add(elevator);
				}
			}
		}
		else if (instance.CompareTag("room"))
		{
			RaycastHit hit;
			Ray rayLeft = new Ray(instance.transform.position, Vector3.left);
			var room = instance.GetComponent<RoomScript>();
			RoomScript leftRoom = null;
			RoomScript rightRoom = null;
			Elevator leftElevator = null;
			Elevator rightElevator = null;
			if (Physics.Raycast(rayLeft, out hit, 6f))
			{
				if (hit.collider.CompareTag("room"))
				{
					leftRoom = hit.collider.GetComponent<RoomScript>();
					room.connectedElevators.AddRange(leftRoom.connectedElevators);
				}
				else if (hit.collider.CompareTag("elevator"))
				{
					leftElevator = hit.collider.GetComponent<Elevator>();
					room.connectedElevators.Add(leftElevator);
				}
			}
			Ray rayRight = new Ray(instance.transform.position, Vector3.right);
			if (Physics.Raycast(rayLeft, out hit, 12f))
			{
				if (hit.collider.CompareTag("room"))
				{
					rightRoom = hit.collider.GetComponent<RoomScript>();
					room.connectedElevators.AddRange(rightRoom.connectedElevators);
				}
				else if (hit.collider.CompareTag("elevator"))
				{
					rightElevator = hit.collider.GetComponent<Elevator>();
					room.connectedElevators.Add(rightElevator);
				}
			}
			if (rightElevator != null)
			{
				rightElevator.connectedElevators.AddRange(room.connectedElevators);
				rightElevator.connectedRooms.Add(room);
			}
			if (leftElevator != null)
			{
				leftElevator.connectedElevators.AddRange(room.connectedElevators);
				leftElevator.connectedRooms.Add(room);
			}
			if (leftRoom != null)
			{
				leftRoom.connectedElevators = room.connectedElevators;
			}
			if (rightRoom != null)
			{
				rightRoom.connectedElevators = room.connectedElevators;
			}
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
