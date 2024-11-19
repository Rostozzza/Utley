using System.Collections.Generic;
using UnityEngine;
using API.Sevices.Mapper;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using System.Linq.Expressions;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public static RequestManager RequestManager = new RequestManager();
	public Tilemap tilemap;
	public List<GameObject> bears = new List<GameObject>();
	[SerializeField] public UIResourceShower uiResourceShower;
	[SerializeField] public GameObject selectedUnit;
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
		var instance = Instantiate(building, queuedBuildPositon.transform.position-new Vector3(0,0, 3.46f), Quaternion.identity);
		if (instance.CompareTag("elevator"))// trying to build elevator
		{
			Debug.Log("Placing elevator");
			RaycastHit hit;
			Ray rayLeft = new Ray(instance.transform.position, Vector3.left * 6f);

			var elevator = instance.GetComponent<Elevator>();
			if (queuedBuildPositon.transform.parent.parent.CompareTag("elevator"))
			{
				Debug.Log("extending existing elevator");
				elevator = queuedBuildPositon.transform.parent.GetComponentInParent<Elevator>();
				instance.transform.parent = elevator.transform;
				Destroy(instance.GetComponent<Elevator>());
			}
			if (Physics.Raycast(rayLeft, out hit, 6f))
			{
				Debug.Log("Collision on left");
				if (hit.collider.CompareTag("room"))
				{
					for (int i = 0; i < hit.transform.GetComponent<RoomScript>().connectedElevators.Count; i++)
					{
						Elevator e = hit.transform.GetComponent<RoomScript>().connectedElevators[i];
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
			hit = new RaycastHit();
			Ray rayRight = new Ray(instance.transform.position, Vector3.right * 6f);
			if (Physics.Raycast(rayRight, out hit, 6f))
			{
				Debug.Log("Collision on right");
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
			if (queuedBuildPositon.transform.position.x < queuedBuildPositon.transform.parent.parent.position.x)
			{
				instance.transform.Translate(Vector3.left * 4f);
			}
			RaycastHit hit;
			Ray rayLeft = new Ray(instance.transform.position, Vector3.left * 6f);
			Debug.DrawRay(instance.transform.position, Vector3.left * 6f, Color.red, 15f);
			var room = instance.GetComponent<RoomScript>();
			RoomScript leftRoom = null;
			RoomScript rightRoom = null;
			Elevator leftElevator = null;
			Elevator rightElevator = null;
			if (Physics.Raycast(rayLeft, out hit, 6f))
			{
				Debug.Log("Collision on left: " + hit.collider.name);
				if (hit.collider.CompareTag("room"))
				{
					Debug.Log("Room on right");
					leftRoom = hit.collider.GetComponent<RoomScript>();
					room.connectedElevators.AddRange(leftRoom.connectedElevators);
				}
				else if (hit.collider.CompareTag("elevator"))
				{
					Debug.Log("Elevator on left");
					leftElevator = hit.collider.GetComponentInParent<Elevator>();
					room.connectedElevators.Add(leftElevator);
				}
			}
			hit = new RaycastHit();
			Ray rayRight = new Ray(instance.transform.position, Vector3.right * 12f);
			Debug.DrawRay(instance.transform.position, Vector3.right * 12f, Color.red, 15f);
			instance.layer = 2; 
			if (Physics.Raycast(rayRight, out hit, 12f))
			{
				Debug.Log("Collision on right: " + hit.collider.name);
				if (hit.collider.CompareTag("room"))
				{
					rightRoom = hit.collider.GetComponent<RoomScript>();
					room.connectedElevators.AddRange(rightRoom.connectedElevators);
				}
				else if (hit.collider.CompareTag("elevator"))
				{
					rightElevator = hit.collider.GetComponentInParent<Elevator>();
					room.connectedElevators.Add(rightElevator);
				}
			}
			instance.layer = 0;
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
		Debug.Log("кликнули по " + gameObject.tag);
		Debug.Log(gameObject.CompareTag("work_station") && selectedUnit != null);
		if (gameObject.CompareTag("work_station") && selectedUnit != null)
		{
			// add move to work station
			gameObject.GetComponentInParent<RoomScript>().StartWork(selectedUnit);
			//Vector3[] transferWalkPoints = gameObject.GetComponentInParent<RoomScript>().GetWalkPoints();
			//Debug.Log(transferWalkPoints);
			//selectedUnit.GetComponent<UnitScript>().PutWalkPoints(gameObject.GetComponentInParent<RoomScript>().GetWalkPoints());
			return;
		}
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
