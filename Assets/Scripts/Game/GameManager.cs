using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections;
using UnityEngine.Video;
using Unity.VisualScripting;
using UnityEngine.Rendering;

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
	[Header("Building settings")]
	public GameObject buildingScreen;
	public GameObject elevatorBuildingScreen;
	//[SerializeField] private GameObject floorPrefab;
	private GameObject queuedBuildPositon;
	public List<GameObject> workStations; // keeps all workstations to address to them to outline when we choosing unit
	private bool buildingMode;
	public RoomScript selectedRoom;
	[Header("Phases settings")]
	private GameObject skyBG;
	[SerializeField] private List<VideoClip> animatedBackgrounds;
	public Season season;
	public int cycleNumber = 1;
	[Header("Resourves")]
	[SerializeField] private int honey;
	[SerializeField] private int asteriy;
	[SerializeField] private int rawAsterium = 0;
	public int maxBearsAmount;
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
		StartCoroutine(ConstantDurabilityDamager(4));
		StartCoroutine(ConstantEnergohoneyConsumer());
		StartCoroutine(ConstantSeasonChanger());

		// DONT FORGET TO DELETE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		asteriy = 100;
		honey = 100;
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
		if (selectedRoom != null)
		{
			selectedRoom.ToggleRoomStats(false);
		}
		selectedRoom = null;
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
		honey = Mathf.Clamp(honey, -1, 999);
	}

	/// <summary>
	/// Changes amount of asterium by given number
	/// </summary>
	/// <param name="amount"></param>
	public void ChangeAsteriy(int amount)
	{
		asteriy += amount;
		asteriy = Mathf.Clamp(asteriy, -1, 999);
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
				selectedUnit = null;
				if (selectedRoom != null)
				{
					selectedRoom.ToggleRoomStats(false);
				}
				selectedRoom = null;
				selectedUnit = null;
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
			if (gameObject.GetComponentInParent<RoomScript>().resource != RoomScript.Resources.Build)
			{
				StartCoroutine(WalkAndStartWork(selectedUnit, gameObject));
			}
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
			if (gameObject.CompareTag("room") && buildingMode)
			{
				if (selectedRoom != null)
				{
					selectedRoom.ToggleRoomStats(false);
					selectedRoom = gameObject.GetComponent<RoomScript>();
					selectedRoom.ToggleRoomStats(true);
				}
				else
				{
					selectedRoom = gameObject.GetComponent<RoomScript>();
					selectedRoom.ToggleRoomStats(true);
				}
			}
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
				if (show && station.GetComponentInParent<RoomScript>().status == RoomScript.Status.Free)
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

	private IEnumerator ConstantDurabilityDamager(int n)
	{
		while (true)
		{
			yield return new WaitForSeconds(10);
			List<GameObject> interestRooms = new List<GameObject>();
			foreach (GameObject room in allRooms)
			{
				if (room.TryGetComponent<RoomScript>(out RoomScript a) && a.status != RoomScript.Status.Destroyed)
				{
					interestRooms.Add(room);
				}
			}
			List<GameObject> shuffleRooms = new List<GameObject>();
			if (n > interestRooms.Count)
			{
				foreach (GameObject room in interestRooms)
				{
					shuffleRooms.Add(room);
				}
				//shuffleRooms = interestRooms.GetRange(0, interestRooms.Count);
				Debug.Log(interestRooms.Count + "|" + shuffleRooms.Count);
			}
			else
			{
				foreach (GameObject room in interestRooms)
				{
					if (shuffleRooms.Count == 0)
					{
						shuffleRooms.Add(room);
					}
					else
					{
						shuffleRooms.Insert(Random.Range(0, shuffleRooms.Count), room);
					}
				}
				shuffleRooms.RemoveRange(n, shuffleRooms.Count - n);
			}
			//foreach (GameObject room in shuffleRooms)
			//{
			//	Debug.Log(room.name + "|" + room);
			//	room.GetComponent<RoomScript>().ChangeDurability(-0.01f);
			//}
			// analogue
			shuffleRooms.ForEach(delegate(GameObject room)
			{
				room.GetComponent<RoomScript>().ChangeDurability(-0.01f);
			});
		}
	}

	private IEnumerator ConstantEnergohoneyConsumer()
	{
		while (true)
		{
			yield return new WaitForSeconds(60);
			int n1 = 0, n2 = 0, n3 = 0;
			foreach (GameObject room in allRooms)
			{
				if (room.TryGetComponent<RoomScript>(out RoomScript roomScript))
				{
					switch (roomScript.level)
					{
						case 1:
							n1++;
							break;
						case 2:
							n2++;
							break;
						case 3:
							n3++;
							break;
					}
				}
			}
			int honeyToEat = (int)(5 + n1 + 1.1 * n2 + 1.2 * n3);
			if (season == Season.Freeze)
			{
				honeyToEat = (int)((float)honeyToEat * (1f + 0.1f + 0.05f * cycleNumber));
			}
			Debug.Log("Съели мёда: " + honeyToEat);
			ChangeHoney(-honeyToEat);
			uiResourceShower.UpdateIndicators();
		}
	}

	private IEnumerator ConstantSeasonChanger()
	{
		while (true)
		{
			yield return new WaitForSeconds(75);
			Debug.Log("Season Changed");
			switch (season)
			{
				case Season.Tide:
					ChangeSeason(Season.Calm);
					cycleNumber++;
					break;
				case Season.Calm:
					ChangeSeason(Season.Storm);
					break;
				case Season.Storm:
					ChangeSeason(Season.Freeze);
					break;
				case Season.Freeze:
					ChangeSeason(Season.Tide);
					DamageRoomsBySeason();
					break;
			}
		}
	}

	public void DamageRoomsBySeason()
	{
		int n = Random.Range(3, 7);
		List<GameObject> interestRooms = new List<GameObject>();
		foreach (GameObject room in allRooms)
		{
			if (room.TryGetComponent<RoomScript>(out RoomScript a) && a.status != RoomScript.Status.Destroyed)
			{
				interestRooms.Add(room);
			}
		}
		List<GameObject> shuffleRooms = new List<GameObject>();
		if (n > interestRooms.Count)
		{
			foreach (GameObject room in interestRooms)
			{
				shuffleRooms.Add(room);
			}
		}
		else
		{
			foreach (GameObject room in interestRooms)
			{
				if (shuffleRooms.Count == 0)
				{
					shuffleRooms.Add(room);
				}
				else
				{
					shuffleRooms.Insert(Random.Range(0, shuffleRooms.Count), room);
				}
			}
			shuffleRooms.RemoveRange(n, shuffleRooms.Count - n);
		}
		shuffleRooms.ForEach(delegate(GameObject room)
		{
			float damage = (0.35f + 0.04f * cycleNumber - 0.02f * room.GetComponent<RoomScript>().depthLevel) * room.GetComponent<RoomScript>().durability;
			Debug.Log(room.name + " задамажен фазой на " + damage);
			room.GetComponent<RoomScript>().ChangeDurability(-damage);
		});
	}

	public enum Season
	{
		Calm,
		Storm,
		Freeze,
		Tide
	}
}
