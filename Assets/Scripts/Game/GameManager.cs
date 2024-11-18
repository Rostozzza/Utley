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

	public void QueueBuildPos(GameObject queuePos)
	{
		queuedBuildPositon = queuePos;
		buildingScreen.SetActive(true);
	}

	public void SelectAndBuild(GameObject building)
	{
		var instance = Instantiate(building, queuedBuildPositon.transform.position, Quaternion.identity);
		if (instance.CompareTag("elevator"))
		{
			RaycastHit hit;
			Ray rayLeft = new Ray(instance.transform.position,Vector3.left);
			var elevator = instance.GetComponent<Elevator>();
			if (queuedBuildPositon.transform.parent.parent.CompareTag("room"))
			{
				
			}
			if (Physics.Raycast(rayLeft, out hit, 6f))
			{
				if (hit.collider.CompareTag("room"))
				{
					foreach (var e in hit.transform.GetComponent<RoomScript>().connectedElevators)
					{
						e.connectedElevators.Add(instance.GetComponent<Elevator>());
						instance.GetComponent<Elevator>().connectedElevators.Add(e);
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
						e.connectedElevators.Add(instance.GetComponent<Elevator>());
						instance.GetComponent<Elevator>().connectedElevators.Add(e);
					}
					hit.transform.GetComponent<RoomScript>().connectedElevators.Add(instance.GetComponent<Elevator>());
					instance.GetComponent<Elevator>().connectedRooms.Add(hit.transform.GetComponent<RoomScript>());
					instance.GetComponent<Elevator>().connectedElevators.Add(instance.GetComponent<Elevator>());
				}
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
