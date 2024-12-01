using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using UnityEngine.Video;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
	[Header("Player settings")]
	public string playerName;
	public Player playerModel;
	[Header("Cosmodrome settings")]
	[SerializeField] private GameObject ui;
	[SerializeField] private List<Image> asteriumRoomView;
	[SerializeField] private List<RoomScript> asteriumRooms;
	public List<GameObject> allRooms;
	[SerializeField] private Transform asteriumViewGrid;
	[SerializeField] private GameObject asteriumViewPrefab;
	[Header("GameManager settings")]
	public static GameManager Instance;
	public JsonManager JsonManager = new JsonManager();
	public RequestManager RequestManager = new RequestManager();
	public GameObject elevatorPrefab;
	public List<GameObject> bears = new List<GameObject>();
	[SerializeField] public UIResourceShower uiResourceShower;
	[SerializeField] public GameObject selectedUnit;
	[SerializeField] private GameObject emptyBearPrefab;
	[Header("Building settings")]
	public GameObject buildingScreen;
	public GameObject elevatorBuildingScreen;
	//[SerializeField] private GameObject floorPrefab;
	private GameObject queuedBuildPositon;
	public List<GameObject> workStations; // keeps all workstations to address to them to outline when we choosing unit
	public List<GameObject> builderRooms;
	private bool buildingMode;
	public GameObject fixedBuilderRoom;
	public RoomScript selectedRoom;
	[SerializeField] private List<GameObject> allPossibleRooms;
	[Header("Phases settings")]
	private GameObject skyBG;
	[SerializeField] private List<VideoClip> animatedBackgrounds;
	public Season season;
	public Mode mode;
	public int cycleNumber = 1;
	[Header("Resourсes")]
	[SerializeField] private int honey;
	[SerializeField] private int asteriy;
	[SerializeField] private int rawAsterium = 0;
	public int maxBearsAmount;
	RaycastHit raycastHit;

	public void LoadBearFromModel(Bear model)
	{
		var newBear = Instantiate(emptyBearPrefab);
		newBear.GetComponent<UnitScript>().LoadDataFromModel(model);
		bears.Add(newBear);
	}

	public void DebugPlayerSave()
	{
		JsonManager.SavePlayerToJson("Obama");
	}

	public void DebugLoadPlayer(string player)
	{
		JsonManager.LoadPlayerFromModel(JsonConvert.DeserializeObject<Player>(player));
	}

	public bool LoadRoomFromModel(Room room)
	{
		try
		{
			var newRoom = Instantiate(allPossibleRooms.First(x => room.Type.Contains(x.gameObject.name)), new Vector3(room.Coordinates[0], room.Coordinates[1], room.Coordinates[2]), Quaternion.identity);
			newRoom.GetComponent<RoomScript>().roomModel = room;
			allRooms.Add(newRoom);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public bool LoadElevatorFromModel(ElevatorModel model)
	{
		try
		{
			var newElevator = Instantiate(elevatorPrefab, new Vector3(model.Coordinates[0], model.Coordinates[1], model.Coordinates[2]), Quaternion.identity);
			newElevator.GetComponent<Elevator>().elevatorModel = model;
			allRooms.Add(newElevator);
			for (int i = 0; i < model.BlocksUp; i++)
			{
				var newBlock = Instantiate(elevatorPrefab, new Vector3(model.Coordinates[0], model.Coordinates[1], model.Coordinates[2]), Quaternion.identity);
				Destroy(newBlock.GetComponent<Elevator>());
				newBlock.transform.parent = newElevator.transform;
				newBlock.transform.Translate(new Vector3(0,(i+1) * 4,0));
			}
			for (int i = 0; i < model.BlocksDown; i++)
			{
				var newBlock = Instantiate(elevatorPrefab, new Vector3(model.Coordinates[0], model.Coordinates[1], model.Coordinates[2]), Quaternion.identity);
				Destroy(newBlock.GetComponent<Elevator>());
				newBlock.transform.parent = newElevator.transform;
				newBlock.transform.Translate(new Vector3(0, (i + 1) * -4, 0));
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public void AssembleBase()
	{
		var roomScripts = allRooms.Where(x => x.GetComponent<RoomScript>()).ToList();
		var elevatorScripts = allRooms.Where(x => x.GetComponent<Elevator>()).ToList();
		foreach (GameObject room in roomScripts)
		{
			var roomScript = room.GetComponent<RoomScript>();
			roomScript.connectedElevators = roomScript.roomModel.ConnectedElevators.Select(x => allRooms[x].GetComponent<Elevator>()).ToList();
		}
		foreach (GameObject elevator in elevatorScripts)
		{
			var elevatorScript = elevator.GetComponent<Elevator>();
			elevatorScript.connectedElevators = elevatorScript.elevatorModel.ConnectedElevators.Select(x => allRooms[x].GetComponent<Elevator>()).ToList();
			elevatorScript.connectedRooms = elevatorScript.elevatorModel.ConnectedRooms.Select(x => allRooms[x].GetComponent<RoomScript>()).ToList();
		}
	}

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			//DontDestroyOnLoad(gameObject);
		}
		skyBG = GameObject.FindGameObjectWithTag("skyBG");
		ChangeSeason(season);
		StartCoroutine(ConstantDurabilityDamager(4));
		StartCoroutine(ConstantEnergohoneyConsumer());
		StartCoroutine(ConstantSeasonChanger());
		ChangeSeason(Season.Calm);
		SetModeByButton(1);
		SetModeByButton(1);
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
	public void SetMode(Mode mode)
	{
		this.mode = mode;
		selectedUnit = null;
		if (selectedRoom)
		{
			selectedRoom.ToggleRoomStats(false);
			selectedRoom.ToggleBuildStats(false);
		}
		selectedRoom = null;
		foreach (GameObject room in allRooms)
		{
			foreach (var button in room.GetComponentsInChildren<Button>(true))
			{
				button.gameObject.SetActive(mode == Mode.Build);
			}
		}
		if (mode == Mode.Build)
		{
			Camera.main.GetComponent<CameraController>().CameraMove();
		}
		else
		{
			queuedBuildPositon = null;
			buildingScreen.SetActive(false);
			elevatorBuildingScreen.SetActive(false);
		}
		if (mode == Mode.Info)
		{
		}
		else
		{
			bears.ForEach(x => x.GetComponent<UnitScript>().SetStatsScreen(false));
		}
	}

	public void SetMode(int mode)
	{
		SetMode((Mode)mode);
	}

	public void SetModeByButton(int mode)
	{
		if (this.mode == (Mode)mode)
		{
			SetMode(Mode.None);
		}
		else
		{
			SetMode((Mode)mode);
		}
	}

	/// <summary>
	/// Builds a room from variable "building" at position of "queuedBuildPosition"
	/// </summary>
	/// <param name="building"></param>
	public async void SelectAndBuild(GameObject building)
	{
		if (await GetAsteriy() < 60)
		{
			queuedBuildPositon = null;
			buildingScreen.SetActive(false);
			elevatorBuildingScreen.SetActive(false);
			return;
		}
		ChangeAsteriy(-60);
		uiResourceShower.UpdateIndicators();

		// goto room vvvvv
		fixedBuilderRoom = null;
		foreach (GameObject room in builderRooms)
		{
			if ((room.GetComponent<RoomScript>().status == RoomScript.Status.Free) && room.GetComponent<RoomScript>().fixedBear)
			{
				room.GetComponent<RoomScript>().SetStatus(RoomScript.Status.Busy);
				fixedBuilderRoom = room;
				break;
			}
		}
		if (fixedBuilderRoom == null)
		{
			Debug.Log("Нет свободных строительных комплексов!");
			queuedBuildPositon = null;
			buildingScreen.SetActive(false);
			elevatorBuildingScreen.SetActive(false);
			return;
		}
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().StopAllCoroutines();
		if (building.CompareTag("elevator") && queuedBuildPositon.transform.parent.parent.CompareTag("elevator"))
		{
			float minLen = Vector3.Distance(queuedBuildPositon.transform.parent.GetComponentInParent<Elevator>().connectedRooms[0].transform.position, queuedBuildPositon.transform.position);
			RoomScript nearestRoom = queuedBuildPositon.transform.parent.GetComponentInParent<Elevator>().connectedRooms.OrderBy(x => x.transform.position.y).ToList()[0];
			fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().MoveToRoom(nearestRoom);
		}
		else
		{
			if (queuedBuildPositon.transform.parent.parent.transform.CompareTag("elevator"))
			{
				float minLen = Vector3.Distance(queuedBuildPositon.transform.parent.GetComponentInParent<Elevator>().connectedRooms[0].transform.position, queuedBuildPositon.transform.position);
				RoomScript nearestRoom = queuedBuildPositon.transform.parent.GetComponentInParent<Elevator>().connectedRooms.OrderBy(x => x.transform.position.y).ToList()[0];
				fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().MoveToRoom(nearestRoom);
			}
			else
			{
				fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().MoveToRoom(queuedBuildPositon.transform.parent.parent.GetComponent<RoomScript>());
			}
		}
		StartCoroutine(SelectAndBuildWaiter(building, fixedBuilderRoom, queuedBuildPositon.transform));
		buildingScreen.SetActive(false);
		elevatorBuildingScreen.SetActive(false);
	}

	private IEnumerator SelectAndBuildWaiter(GameObject building, GameObject room, Transform point)
	{
		while (room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoutine != null)
		{
			yield return null;
		}
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", true);
		yield return new WaitForSeconds(5);
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", false);
		room.GetComponent<RoomScript>().SetStatus(RoomScript.Status.Free);
		SelectAndBuildMainBlock(building, point);
		room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CanBeSelected();
	}

	private async void SelectAndBuildMainBlock(GameObject building, Transform point)
	{
		var instance = Instantiate(building, point.position - new Vector3(0,0, 3.46f - 5f), Quaternion.identity);
		if (instance.CompareTag("elevator"))// trying to build elevator
		{
			Debug.Log("Placing elevator");
			RaycastHit hit;
			Ray rayLeft = new Ray(instance.transform.position, Vector3.left * 6f);

			var elevator = instance.GetComponent<Elevator>();
			if (point.parent.parent.CompareTag("elevator"))
			{
				Debug.Log("extending existing elevator");
				elevator = point.parent.GetComponentInParent<Elevator>();
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
			if (point.position.x < point.parent.parent.position.x)
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
				var newAsteriumView = Instantiate(asteriumViewPrefab, asteriumViewGrid);
				asteriumRoomView.Add(newAsteriumView.GetComponent<Image>());
			}
		}
		if (queuedBuildPositon.transform.parent.parent.tag=="elevator" && queuedBuildPositon.transform.parent.parent.position.y != instance.transform.position.y && instance.tag == "elevator")
		{
			queuedBuildPositon = null;
			return;
		}
		queuedBuildPositon = null;
		allRooms.Add(instance);
		await JsonManager.SavePlayerToJson(playerName);
	}

	/// <summary>
	/// Returns amount of honey on current client
	/// </summary>
	/// <returns></returns>
	public async Task<int> GetHoney()
	{
		var model = await RequestManager.GetPlayer(playerName);
		playerModel = model;
		return int.Parse(model.resources["honey"]);
	}

	/// <summary>
	/// Returns amount of Asteriun on current client
	/// </summary>
	/// <returns></returns>
	public async Task<int> GetAsteriy()
	{
		var model = await RequestManager.GetPlayer(playerName);
		playerModel = model;
		return int.Parse(model.resources["honey"]);
	}

	/// <summary>
	/// Changes amount of honey by given number
	/// </summary>
	/// <param name="amount"></param>
	public async void ChangeHoney(int amount)
	{
		int serverHoney = await GetHoney();
		serverHoney += amount;
		serverHoney = Mathf.Clamp(serverHoney, -1, 999);
		honey = serverHoney;
		await JsonManager.SavePlayerToJson(playerName);
	}

	/// <summary>
	/// Changes amount of asterium by given number
	/// </summary>
	/// <param name="amount"></param>
	public async void ChangeAsteriy(int amount)
	{
		int serverAsterium = await GetAsteriy();
		serverAsterium += amount;
		serverAsterium = Mathf.Clamp(serverAsterium, -1, 999);
		asteriy = serverAsterium;
		await JsonManager.SavePlayerToJson(playerName);
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
		asteriumRooms[rawAsterium - 1].isReadyForWork = true;
		asteriumRooms[rawAsterium - 1].StartWork(gameObject);
		asteriumRoomView[rawAsterium - 1].color = Color.blue;
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
			if (Physics.Raycast(ray, out raycastHit, 100f) && !buildingScreen.activeSelf)
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
					selectedRoom.ToggleBuildStats(false);
				}
				selectedRoom = null;
				selectedUnit = null;
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out raycastHit, 100f) && !buildingScreen.activeSelf)
			{
				if (raycastHit.transform != null)
				{
					RightClick(raycastHit.transform.gameObject);
					OutlineWorkStations(false);
				}
			}
		}
		else if (Input.GetKeyDown(KeyCode.Escape) && (buildingScreen.activeSelf || elevatorBuildingScreen.activeSelf))
		{
			buildingScreen.SetActive(false);
			elevatorBuildingScreen.SetActive(false);
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
			else
			{
				gameObject.GetComponentInParent<RoomScript>().fixedBear = selectedUnit;
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

		if (gameObject.CompareTag("unit"))
		{
			if (gameObject.GetComponent<UnitScript>().selectable)
			{
				switch (mode)
				{
					case Mode.None:
						selectedUnit = gameObject;
						OutlineWorkStations(true);
						break;
					case Mode.Info:
						gameObject.GetComponent<UnitScript>().SetStatsScreen();
						break;
				}
			}
		}
		else
		{
			//selectedUnit.GetComponent<UnitScript>().SetStatsScreen(false);
			selectedUnit = null;
			OutlineWorkStations(false);
			if (gameObject.CompareTag("room"))
			{
				switch (mode)
				{
					case Mode.Build:
						if (selectedRoom != null)
						{
							selectedRoom.ToggleBuildStats(false);
							selectedRoom = gameObject.GetComponent<RoomScript>();
							selectedRoom.ToggleBuildStats(true);
						}
						else
						{
							selectedRoom = gameObject.GetComponent<RoomScript>();
							selectedRoom.ToggleBuildStats(true);
						}
						break;
					case Mode.Info:
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
						break;
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
		vp.clip = animatedBackgrounds[(int)season];
		if (!vp.isPlaying)
		{
			vp.Play();
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
				bear.GetComponent<UnitScript>().Boost();
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
				bearToBoost.GetComponent<UnitScript>().Boost();
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
				bearToBoost.GetComponent<UnitScript>().Boost();
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
				bearToBoost.GetComponent<UnitScript>().Boost();
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
			shuffleRooms.ForEach(delegate (GameObject room)
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
			
		}
	}

	private async void ConsumeEnergohoney()
	{
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
		var model = JsonManager.SavePlayerToJson(playerName);
		Dictionary<string, int> changedResources = new Dictionary<string, int>();
		changedResources.Add("honey", -honeyToEat);
		JsonManager.CreateLog(new Log
		{
			Comment = $"Complex deplicted {honeyToEat} energohoney",
			PlayerName = playerName,
			ShopName = null,
			ResourcesChanged = changedResources
		});
		Debug.Log("Съели мёда: " + honeyToEat);
		ChangeHoney(-honeyToEat);
		uiResourceShower.UpdateIndicators();
	}

	private IEnumerator ConstantSeasonChanger()
	{
		while (true)
		{
			yield return new WaitForSeconds(75); //default 75
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
		shuffleRooms.ForEach(delegate (GameObject room)
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

	public enum Mode
	{
		None,
		Build,
		Info
	}
}