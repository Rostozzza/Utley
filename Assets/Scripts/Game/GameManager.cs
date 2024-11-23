using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
	[Header("Cosmodrome settings")]
	[SerializeField] private GameObject ui;
	[SerializeField] private List<Image> asteriumRoomView;
	[SerializeField] private List<RoomScript> asteriumRooms;
	[SerializeField] private List<GameObject> allRooms;
	[SerializeField] private Transform asteriumViewGrid;
	[SerializeField] private GameObject asteriumViewPrefab;
	[Header("GameManager settings")]
	public static GameManager Instance;
	public static RequestManager RequestManager = new RequestManager();
	public Tilemap tilemap;
	public List<GameObject> bears = new List<GameObject>();
	[SerializeField] public UIResourceShower uiResourceShower;
	[SerializeField] public GameObject selectedUnit;
	public GameObject buildingScreen;
	public GameObject elevatorBuildingScreen;
	[SerializeField] private GameObject floorPrefab;
	private GameObject queuedBuildPositon;
	public List<GameObject> workStations; // keeps all workstations to address to them to outline when we choosing unit
	private bool buildingMode;
	private GameObject skyBG;
	[SerializeField] private List<VideoClip> animatedBackgrounds;
	public Season season;
	public int maxBearsAmount = 3;

	[SerializeField] private int honey;
	[SerializeField] private int asteriy;
	[SerializeField] private int rawAsterium = 0;

	RaycastHit raycastHit;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		skyBG = GameObject.FindGameObjectWithTag("skyBG");
		ChangeSeason(season);
	}

	/// <summary>
	/// Saves position at "queuedBuildPosition"
	/// </summary>
	/// <param name="queuePos"></param>
	public void QueueBuildPos(GameObject queuePos)
	{
		queuedBuildPositon = queuePos;
		buildingScreen.SetActive(false);
		elevatorBuildingScreen.SetActive(false);
	}

	/// <summary>
	/// Toggles building mode and all buttons;
	/// </summary>
	public void ToggleBuildingMode()
	{
		buildingMode = !buildingMode;
		selectedUnit = null;
		foreach (GameObject room in allRooms)
		{
			foreach(var button in room.GetComponentsInChildren<Button>(true))
			{
				button.gameObject.SetActive(buildingMode);
			}
		}
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
			elevator.connectedElevators = elevator.connectedElevators.Distinct().ToList();
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
				room.connectedElevators = room.connectedElevators.Distinct().ToList();
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
				room.connectedElevators = room.connectedElevators.Distinct().ToList();
			}
			instance.layer = 0;
			if (rightElevator != null)
			{
				rightElevator.connectedElevators.AddRange(room.connectedElevators);
				rightElevator.connectedRooms.Add(room);
				rightElevator.connectedElevators = rightElevator.connectedElevators.Distinct().ToList();
			}
			if (leftElevator != null)
			{
				leftElevator.connectedElevators.AddRange(room.connectedElevators);
				leftElevator.connectedRooms.Add(room);
				leftElevator.connectedElevators = leftElevator.connectedElevators.Distinct().ToList();
			}
			if (leftRoom != null)
			{
				leftRoom.connectedElevators = room.connectedElevators;
			}
			if (rightRoom != null)
			{
				rightRoom.connectedElevators = room.connectedElevators;
			}
			if (room.resource == RoomScript.Resources.Asteriy)
			{
				asteriumRooms.Add(room);
				var newAsteriumView = Instantiate(asteriumViewPrefab,asteriumViewGrid);
				asteriumRoomView.Add(newAsteriumView.GetComponent<Image>());
			}
		}
		allRooms.Add(instance);
		queuedBuildPositon = null;
		buildingScreen.SetActive(false);
		elevatorBuildingScreen.SetActive(false);
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

	public bool FlyForRawAsterium()
	{
		if (rawAsterium < asteriumRoomView.Count)
		{
			return true;
		}
		return false;
	}

	public void DeliverRawAsterium()
	{
		rawAsterium++;
		asteriumRooms[rawAsterium-1].isReadyForWork = true;
		asteriumRooms[rawAsterium - 1].StartWork(gameObject);
		asteriumRoomView[rawAsterium-1].color = Color.blue;
	}

	public void WithdrawRawAsterium()
	{
		rawAsterium--;
		asteriumRoomView[rawAsterium].color = Color.grey;
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
				else
				{
					OutlineWorkStations(false);
				}
			}
			else
			{
				OutlineWorkStations(false);
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
					OutlineWorkStations(false);
				}
			}
		}
	}

	private void RightClick(GameObject gameObject)
	{
		if (gameObject.CompareTag("work_station") && selectedUnit != null)
		{
			StartCoroutine(WalkAndStartWork(selectedUnit, gameObject));
			selectedUnit = null;
			return;
		}
		if (gameObject.CompareTag("room"))
		{
			selectedUnit.GetComponent<UnitMovement>().StopAllCoroutines();
			selectedUnit.GetComponent<UnitMovement>().MoveToRoom(gameObject.GetComponent<RoomScript>());
		}
	}

	private IEnumerator WalkAndStartWork(GameObject unit, GameObject obj) // needs to wait for walk and after we starting work
	{
		unit.GetComponent<UnitMovement>().StopAllCoroutines();
		unit.GetComponent<UnitMovement>().MoveToRoom(obj.GetComponentInParent<RoomScript>());
		while (unit.GetComponent<UnitMovement>().currentRoutine != null)
		{
			yield return null;
		}
		obj.GetComponentInParent<RoomScript>().StartWork(unit);
		OutlineWorkStations(false);
	}

	private void ClickedGameObject(GameObject gameObject)
	{
		Debug.Log("кликнули по " + gameObject.tag);
		
		if (gameObject.CompareTag("unit") && !buildingMode)
		{
			if (gameObject.GetComponent<UnitScript>().selectable)
			{
				selectedUnit = gameObject;
				OutlineWorkStations(true);
			}
		}
		else
		{
			selectedUnit = null;
			OutlineWorkStations(false);
		}
		
		
	}

	/// <summary>
	/// Adds workstations to outline them when unit choosed
	/// </summary>
	/// <param name="stations"></param>
	public void AddWorkStations(List<GameObject> stations)
	{
		workStations.AddRange(stations);
	}

	private void OutlineWorkStations(bool show)
	{
		if (workStations.Count > 0)
		{
			foreach (GameObject station in workStations)
			{
				if (show && station.GetComponentInParent<RoomScript>().status != RoomScript.Status.Busy)
				{
					station.layer = 8;
				}
				else
				{
					station.layer = 0;
				}
			}
		}
	}

    /// <summary>
	/// Changes season with refreshing
	/// </summary>
	/// <param name="season"></param>
	public void ChangeSeason(Season season)
	{
		this.season = season;
		VideoPlayer vp = skyBG.GetComponentInChildren<VideoPlayer>();
		if (season == Season.Calm)
		{
			vp.Stop();
		}
		else
		{
			vp.clip = animatedBackgrounds[(int)season - 1];
			if (!vp.isPlaying)
			{
				vp.Play();
			}
		}
	}

	public void ChangeMaxBearAmount(int amount)
	{
		maxBearsAmount += amount;
		uiResourceShower.UpdateIndicators();
	}

	public void BoostThreeBears()
	{
		if (bears.Count < 3)
		{
			foreach (GameObject bear in bears)
			{
				bear.GetComponent<UnitScript>().isBoosted = true;
			}
		}
		else
		{
			int c;
			GameObject bearToBoost;
			if (bears.GetRange(0, bears.Count / 3).ConvertAll(x => x.GetComponent<UnitScript>().isBoosted).Contains(false))
			{
				c = 0;
				bearToBoost = bears[Random.Range(0, bears.Count / 3)];
				while (bearToBoost.GetComponent<UnitScript>().isBoosted)
				{
					bearToBoost = bears[Random.Range(0, bears.Count / 3)];
					c++;
					if (c > 100)
					{
						break;
					}
				}
				bearToBoost.GetComponent<UnitScript>().isBoosted = true;
			}
			if (bears.GetRange(bears.Count / 3, (bears.Count / 3) * 2).ConvertAll(x => x.GetComponent<UnitScript>().isBoosted).Contains(false))
			{
				c = 0;
				bearToBoost = bears[Random.Range(bears.Count / 3, (bears.Count / 3) * 2)];
				while (bearToBoost.GetComponent<UnitScript>().isBoosted)
				{
					bearToBoost = bears[Random.Range(bears.Count / 3, (bears.Count / 3) * 2)];
					c++;
					if (c > 100)
					{
						break;
					}
				}
				bearToBoost.GetComponent<UnitScript>().isBoosted = true;
			}
			if (bears.GetRange((bears.Count / 3) * 2, bears.Count).ConvertAll(x => x.GetComponent<UnitScript>().isBoosted).Contains(false))
			{
				c = 0;
				bearToBoost = bears[Random.Range((bears.Count / 3) * 2, bears.Count)];
				while (bearToBoost.GetComponent<UnitScript>().isBoosted)
				{
					bearToBoost = bears[Random.Range((bears.Count / 3) * 2, bears.Count)];
					c++;
					if (c > 100)
					{
						break;
					}
				}
				bearToBoost.GetComponent<UnitScript>().isBoosted = true;
			}
			//bears[Random.Range(bears.Count / 3, (bears.Count / 3) * 2)].GetComponent<UnitScript>().isBoosted = true;
			//bears[Random.Range((bears.Count / 3) * 2, bears.Count)].GetComponent<UnitScript>().isBoosted = true;
		}
	}

	public enum Season
	{
		Calm,
		Storm,
		Freeze,
		Blizzard
	}
}
