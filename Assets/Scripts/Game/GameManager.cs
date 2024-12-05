using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
	[Header("Player settings")]
	public string playerName;
	public Player playerModel;
	public bool isAPIActive;
	[Header("Cosmodrome settings")]
	[SerializeField] private GameObject ui;
	[SerializeField] private List<Image> asteriumRoomView;
	[SerializeField] private List<RoomScript> asteriumRooms;
	public List<GameObject> allRooms;
	[SerializeField] private Transform asteriumViewGrid;
	[SerializeField] private GameObject asteriumViewPrefab;
	[Header("GameManager settings")]
	public static GameManager Instance;
	public JsonManager JsonManager;
	public RequestManager RequestManager;
	public GameObject elevatorPrefab;
	public List<GameObject> bears = new List<GameObject>();
	[SerializeField] public UIResourceShower uiResourceShower;
	[SerializeField] public RoomStatusListController roomStatusListController;
	[SerializeField] public BearStatusListController bearStatusListController;
	[SerializeField] public GameObject globalVolume;
	[SerializeField] public GameObject selectedUnit;
	[SerializeField] private GameObject emptyBearPrefab;
	[SerializeField] private Sprite defaultBuildButton;
	[SerializeField] private Sprite selectedBuildButton;
	[SerializeField] private Sprite defaultInfoButton;
	[SerializeField] private Sprite selectedInfoButton;
	[SerializeField] private List<BearStatusController> bearsToMoveOn;
	[SerializeField] private bool isGraphUsing = false;
	[SerializeField] private float seasonTimeLeft;

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
	[SerializeField] private List<GameObject> animatedBackgrounds;
	public Season season;
	public Mode mode;
	public int cycleNumber = 1;
	[Header("Resourсes")]
	[SerializeField] private float honey;
	[SerializeField] private int asteriy;
	[SerializeField] private int rawAsterium = 0;
	public int maxBearsAmount;
	RaycastHit raycastHit;
	[Header("Effects")]
	[SerializeField] private GameObject buildingParticle;

	public void LoadBearFromModel(Bear model)
	{
		var newBear = Instantiate(emptyBearPrefab);
		newBear.GetComponent<UnitScript>().LoadDataFromModel(model);
		bears.Add(newBear);
	}

	public void DebugPlayerSave()
	{
		//JsonManager.SavePlayerToJson("Obama");
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
				newBlock.transform.Translate(new Vector3(0, (i + 1) * 4, 0));
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
		//Time.timeScale = 20;
		if (Instance == null)
		{
			Instance = this;
			//DontDestroyOnLoad(gameObject);
		}
		if (!isAPIActive)
		{
			asteriy = 600;
			honey = 100;
		}
		skyBG = GameObject.FindGameObjectWithTag("skyBG");
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
		globalVolume.GetComponent<Volume>().profile.TryGet(out Vignette vignette);
		this.mode = mode;
		if (selectedUnit != null)
		{
			selectedUnit.GetComponent<UnitScript>().SetMarker(false);
			selectedUnit = null;
		}
		HideAllAssignButtons();
		if (selectedRoom)
		{
			selectedRoom.ToggleRoomStats(false);
			selectedRoom.ToggleBuildStats(false);
		}
		selectedRoom = null;
		foreach (GameObject room in allRooms)
		{
			foreach (var button in room.GetComponentsInChildren<Button>(true).Where(x => !x.CompareTag("assignment")).ToList())
			{
				button.gameObject.SetActive(mode == Mode.Build);
			}
		}
		if (mode == Mode.None)
		{
			vignette.intensity.value = 0f;
		}
		if (mode == Mode.Build)
		{
			vignette.intensity.value = 0.27f;
			vignette.color.value = Color.black;
			Camera.main.GetComponent<CameraController>().CameraMove();
		}
		else
		{
			buildingScreen.SetActive(false);
			elevatorBuildingScreen.SetActive(false);
		}
		if (mode == Mode.Info)
		{
			vignette.intensity.value = 0.27f;
			vignette.color.value = Color.blue;
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
		Image buildButton = GameObject.FindGameObjectWithTag("build_button").GetComponent<Image>();
		Image infoButton = GameObject.FindGameObjectWithTag("info_button").GetComponent<Image>();
		if (this.mode == (Mode)mode)
		{
			switch (this.mode)
			{
				case Mode.Build:
					buildButton.sprite = defaultBuildButton;
					break;
				case Mode.Info:
					infoButton.sprite = defaultInfoButton;
					break;
			}
			SetMode(Mode.None);
		}
		else
		{
			switch (this.mode)
			{
				case Mode.Build:
					buildButton.sprite = defaultBuildButton;
					break;
				case Mode.Info:
					infoButton.sprite = defaultInfoButton;
					break;
			}
			switch ((Mode)mode)
			{
				case Mode.Build:
					buildButton.sprite = selectedBuildButton;
					break;
				case Mode.Info:
					infoButton.sprite = selectedInfoButton;
					break;
			}
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
		fixedBuilderRoom = null;
		foreach (GameObject room in builderRooms)
		{
			if (room.GetComponent<BuilderRoom>().GetWait() && room.GetComponent<BuilderRoom>().fixedBear)
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
		await ChangeAsteriy(-60);
		uiResourceShower.UpdateIndicators();

		// goto room vvvvv
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().StopAllCoroutines();
		fixedBuilderRoom.GetComponent<BuilderRoom>().SetWait(false);
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
		Instantiate(buildingParticle, queuedBuildPositon.transform.position, Quaternion.identity);
		if (!building.CompareTag("elevator"))
		{
			if (queuedBuildPositon.transform.position.x < queuedBuildPositon.transform.parent.parent.position.x)
			{
				Instantiate(buildingParticle, queuedBuildPositon.transform.position + Vector3.left * 4f, Quaternion.identity);
			}
			else
			{
				Instantiate(buildingParticle, queuedBuildPositon.transform.position + Vector3.right * 4f, Quaternion.identity);
			}
		}
		room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(true);
		yield return new WaitForSeconds(5);
		room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().LevelUpBear();
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", false);
		room.GetComponent<RoomScript>().SetStatus(RoomScript.Status.Free);
		SelectAndBuildMainBlock(building, point);
		StartCoroutine(WalkAndStartWork(room.GetComponent<BuilderRoom>().fixedBear, room));
		while (room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoutine != null)
		{
			yield return null;
		}
		room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CanBeSelected();
	}

	private async Task SelectAndBuildMainBlock(GameObject building, Transform point)
	{
		var instance = Instantiate(building, point.position - new Vector3(0, 0, 3.46f - 5f), Quaternion.identity);
		if (instance.CompareTag("elevator"))// trying to build elevator
		{
			Debug.Log("Placing elevator");
			RaycastHit hit;
			Ray rayLeft = new Ray(instance.transform.position, Vector3.left * 6f);

			var elevator = instance.GetComponent<Elevator>();
			if (point.parent.parent.CompareTag("elevator") && !queuedBuildPositon.name.Contains("Gen"))
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
			if (instance.TryGetComponent(out SupplyRoom supply))
			{
				supply.InitializeGraph();
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
		if (queuedBuildPositon.transform.parent.parent.tag == "elevator" && queuedBuildPositon.transform.parent.parent.position.y != instance.transform.position.y && instance.tag == "elevator")
		{
			queuedBuildPositon = null;
			return;
		}
		queuedBuildPositon = null;
		allRooms.Add(instance);
		allRooms.Where(x => x.GetComponent<SupplyRoom>()).ToList().ForEach(y => y.GetComponent<SupplyRoom>().GetRoomsToEnpower());
		if (isAPIActive)
		{
			await JsonManager.SavePlayerToJson(playerName);
		}
	}

	/// <summary>
	/// Returns amount of honey on current client
	/// </summary>
	/// <returns></returns>
	public async Task<float> GetHoney()
	{
		if (isAPIActive)
		{
			var model = await RequestManager.GetPlayer(playerName);
			playerModel = model;
			return int.Parse(model.resources["honey"]);
		}
		return honey;
	}

	/// <summary>
	/// Returns amount of Asteriun on current client
	/// </summary>
	/// <returns></returns>
	public async Task<int> GetAsteriy()
	{
		if (isAPIActive)
		{
			var model = await RequestManager.GetPlayer(playerName);
			playerModel = model;
			return int.Parse(model.resources["asterium"]);
		}
		return asteriy;
	}

	/// <summary>
	/// Changes amount of honey by given number
	/// </summary>
	/// <param name="amount"></param>
	public async Task ChangeHoney(float amount)
	{
		if (isAPIActive)
		{
			float serverHoney = await GetHoney();
			serverHoney += amount;
			serverHoney = Mathf.Clamp(serverHoney, 0, 999);
			honey = serverHoney;
			await JsonManager.SavePlayerToJson(playerName);
			return;
		}
		honey += amount;
		honey = Mathf.Clamp(honey, 0, 999);
	}

	/// <summary>
	/// Changes amount of asterium by given number
	/// </summary>
	/// <param name="amount"></param>
	public async Task ChangeAsteriy(int amount)
	{
		if (isAPIActive)
		{
			int serverAsterium = await GetAsteriy();
			serverAsterium += amount;
			serverAsterium = Mathf.Clamp(serverAsterium, 0, 999);
			asteriy = serverAsterium;
			await JsonManager.SavePlayerToJson(playerName);
			return;
		}
		asteriy += amount;
		asteriy = Mathf.Clamp(asteriy, 0, 999);
	}

	public bool FlyForRawAsterium() => rawAsterium < asteriumRooms.Where(x => x.GetComponent<RoomScript>().isEnpowered).ToList().Count;

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
		if (Input.GetMouseButtonDown(0)) //off
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
					bears.ForEach(x => x.GetComponent<UnitScript>().SetMarker(false));
					HideAllAssignButtons();
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
		else if (Input.GetMouseButtonDown(1)) //off
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

		if (Input.GetKeyDown(KeyCode.Alpha1) && !isGraphUsing)
		{
			if (selectedUnit != bearsToMoveOn[0].GetBearObj())
			{
				bearsToMoveOn[0].MoveToObject();
			}
			else
			{
				selectedUnit.GetComponent<UnitScript>().SetMarker(false);
				selectedUnit.GetComponent<UnitScript>().GetStatusPanel().SetSelect(false);
				HideAllAssignButtons();
				selectedUnit = null;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2) && !isGraphUsing)
		{
			if (selectedUnit != bearsToMoveOn[1].GetBearObj())
			{
				bearsToMoveOn[1].MoveToObject();
			}
			else
			{
				selectedUnit.GetComponent<UnitScript>().SetMarker(false);
				selectedUnit.GetComponent<UnitScript>().GetStatusPanel().SetSelect(false);
				HideAllAssignButtons();
				selectedUnit = null;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3) && !isGraphUsing)
		{
			if (selectedUnit != bearsToMoveOn[2].GetBearObj())
			{
				bearsToMoveOn[2].MoveToObject();
			}
			else
			{
				selectedUnit.GetComponent<UnitScript>().SetMarker(false);
				selectedUnit.GetComponent<UnitScript>().GetStatusPanel().SetSelect(false);
				HideAllAssignButtons();
				selectedUnit = null;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4) && !isGraphUsing)
		{
			if (selectedUnit != bearsToMoveOn[3].GetBearObj())
			{
				bearsToMoveOn[3].MoveToObject();
			}
			else
			{
				selectedUnit.GetComponent<UnitScript>().SetMarker(false);
				selectedUnit.GetComponent<UnitScript>().GetStatusPanel().SetSelect(false);
				HideAllAssignButtons();
				selectedUnit = null;
			}
		}

		if (Input.GetKeyDown(KeyCode.B)) SetModeByButton((int)Mode.Build);
		else if (Input.GetKeyDown(KeyCode.I)) SetModeByButton((int)Mode.Info);
	}

	private bool MouseOnTarget(GameObject target, bool isTutorial)
	{
		if (!isTutorial) return true;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out raycastHit, 100f))
		{
			if (raycastHit.transform != null)
			{
				return target == raycastHit.transform.gameObject;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	private void RightClick(GameObject gameObject)
	{
		//if (gameObject.CompareTag("work_station") && selectedUnit != null)
		//{
		//	if (gameObject.GetComponentInParent<RoomScript>().resource == RoomScript.Resources.Build)
		//	{
		//		gameObject.GetComponentInParent<RoomScript>().fixedBear = selectedUnit;
		//	}
		//	StartCoroutine(WalkAndStartWork(selectedUnit, gameObject));
		//	if (selectedUnit) selectedUnit.GetComponent<UnitScript>().SetMarker(false);
		//	selectedUnit = null;
		//	return;
		//}
		if (gameObject.CompareTag("room"))
		{
			selectedUnit.GetComponent<UnitMovement>().StopAllCoroutines();
			selectedUnit.GetComponent<UnitMovement>().MoveToRoom(gameObject.GetComponent<RoomScript>());
			if (builderRooms.Any(x => x.GetComponent<BuilderRoom>().fixedBear == selectedUnit) && builderRooms.Where(x => x.GetComponent<BuilderRoom>().fixedBear == selectedUnit).ToList()[0].GetComponent<BuilderRoom>().GetWait())
			{
				if (builderRooms.Any(x => x.GetComponent<BuilderRoom>().fixedBear == selectedUnit))
				{
					builderRooms.Where(x => x.GetComponent<BuilderRoom>().fixedBear == selectedUnit).ToList()[0].GetComponent<BuilderRoom>().SetWait(false, true);
					//builderRooms.Where(x => x.GetComponent<BuilderRoom>().fixedBear == unit).ToList()[0].GetComponent<BuilderRoom>().InterruptWork();
				}
			}
		}
	}

	public void WalkAndWork(GameObject unit, GameObject obj)
	{
		StartCoroutine(WalkAndStartWork(unit, obj));
	}

	private IEnumerator WalkAndStartWork(GameObject unit, GameObject obj) // needs to wait for walk and after we starting work
	{
		unit.GetComponent<UnitMovement>().StopAllCoroutines();
		unit.GetComponent<UnitMovement>().MoveToRoom(obj.GetComponentInParent<RoomScript>());
		if (selectedUnit) selectedUnit.GetComponent<UnitScript>().SetMarker(false);

		selectedUnit = null;
		while (unit.GetComponent<UnitMovement>().currentRoutine != null)
		{
			yield return null;
		}
		if (obj.GetComponentInParent<RoomScript>().resource == RoomScript.Resources.Build) obj.GetComponentInParent<BuilderRoom>().SetWait(true);
		obj.GetComponentInParent<RoomScript>().StartWork(unit);
		unit.GetComponent<UnitScript>().SetMarker(false);
		OutlineWorkStations(false);
	}

	public void HideAllAssignButtons()
	{
		allRooms.Where(x => x.GetComponent<RoomScript>()).ToList().ForEach(y => y.GetComponent<RoomScript>().HideButton());
	}

	public void ShowAvailableAssignments()
	{
		allRooms.Where(x => x.GetComponent<RoomScript>()).ToList().ForEach(y => y.GetComponent<RoomScript>().ShowButton());
	}

	public void ClickedGameObject(GameObject gameObject)
	{
		Debug.Log("кликнули по " + gameObject.tag);
		bears.ForEach(x => x.GetComponent<UnitScript>().SetMarker(false));
		if (gameObject.CompareTag("unit"))
		{
			if (gameObject.GetComponent<UnitScript>().selectable)
			{
				switch (mode)
				{
					case Mode.None:
						gameObject.GetComponent<UnitScript>().SelectUnit();
						break;
					case Mode.Info:
						SetModeByButton((int)Mode.None);
						gameObject.GetComponent<UnitScript>().SelectUnit();
						//gameObject.GetComponent<UnitScript>().SetStatsScreen();
						break;
					case Mode.Build:
						SetModeByButton((int)Mode.None);
						gameObject.GetComponent<UnitScript>().SelectUnit();
						break;
				}
			}
		}
		else
		{
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
			else if (gameObject.CompareTag("work_station") && mode == Mode.Info)
			{
				if (selectedRoom != null)
				{
					selectedRoom.ToggleRoomStats(false);
					selectedRoom = gameObject.GetComponentInParent<RoomScript>();
					selectedRoom.ToggleRoomStats(true);
				}
				else
				{
					selectedRoom = gameObject.GetComponentInParent<RoomScript>();
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
		if (this.season == season)
		{
			SpriteRenderer sprite = animatedBackgrounds[(int)season].GetComponent<SpriteRenderer>();
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
		}
		else
		{
			StartCoroutine(SmoothTransition(animatedBackgrounds[(int)this.season], animatedBackgrounds[(int)season]));
		}
		this.season = season;
	}

	private IEnumerator SmoothTransition(GameObject previous, GameObject next)
	{
		yield return StartCoroutine(TransitionShow(next));
		yield return StartCoroutine(TransitionHide(previous));
	}

	private IEnumerator TransitionShow(GameObject obj)
	{
		float timer = 0;
		do
		{
			SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(0, 1, timer + Time.deltaTime));
			timer += Time.deltaTime;
			yield return null;
		} while (timer < 1);
	}

	private IEnumerator TransitionHide(GameObject obj)
	{
		float timer = 0;
		do
		{
			SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(1, 0, timer + Time.deltaTime));
			timer += Time.deltaTime;
			yield return null;
		} while (timer < 1);
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
			//if (bears.GetRange(0, bears.Count / 3).ConvertAll(x => x.GetComponent<UnitScript>().isBoosted).Contains(false))
			//{
			//	c = 0;
			//	bearToBoost = bears[Random.Range(0, bears.Count / 3)];
			//	while (bearToBoost.GetComponent<UnitScript>().isBoosted)
			//	{
			//		bearToBoost = bears[Random.Range(0, bears.Count / 3)];
			//		c++;
			//		if (c > 100)
			//		{
			//			break;
			//		}
			//	}
			//	bearToBoost.GetComponent<UnitScript>().Boost();
			//}
			//if (bears.GetRange(bears.Count / 3, (bears.Count / 3) * 2).ConvertAll(x => x.GetComponent<UnitScript>().isBoosted).Contains(false))
			//{
			//	c = 0;
			//	bearToBoost = bears[Random.Range(bears.Count / 3, (bears.Count / 3) * 2)];
			//	while (bearToBoost.GetComponent<UnitScript>().isBoosted)
			//	{
			//		bearToBoost = bears[Random.Range(bears.Count / 3, (bears.Count / 3) * 2)];
			//		c++;
			//		if (c > 100)
			//		{
			//			break;
			//		}
			//	}
			//	bearToBoost.GetComponent<UnitScript>().Boost();
			//}
			//if (bears.GetRange((bears.Count / 3) * 2, bears.Count).ConvertAll(x => x.GetComponent<UnitScript>().isBoosted).Contains(false))
			//{
			//	c = 0;
			//	bearToBoost = bears[Random.Range((bears.Count / 3) * 2, bears.Count)];
			//	while (bearToBoost.GetComponent<UnitScript>().isBoosted)
			//	{
			//		bearToBoost = bears[Random.Range((bears.Count / 3) * 2, bears.Count)];
			//		c++;
			//		if (c > 100)
			//		{
			//			break;
			//		}
			//	}
			//	bearToBoost.GetComponent<UnitScript>().Boost();
			//}
			bears.ForEach(x => x.GetComponent<UnitScript>().Boost());

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
				if (room.TryGetComponent<RoomScript>(out RoomScript a) && a.status != RoomScript.Status.Destroyed && a.resource != RoomScript.Resources.Build)
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
			yield return new WaitForSeconds(1);
			ConsumeEnergohoney();
		}
	}

	private async void ConsumeEnergohoney()
	{
		int n1 = 0, n2 = 0, n3 = 0;
		foreach (GameObject room in allRooms.Where(x => x.TryGetComponent(out RoomScript roomSctipt) && roomSctipt.isEnpowered))
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
		float honeyToEat = (float)(5 + n1 + 1.05 * n2 + 1.1 * n3) / 60f;
		if (season == Season.Freeze)
		{
			honeyToEat *= 1f + 0.1f + 0.05f * cycleNumber;
		}
		if (isAPIActive)
		{
			var model = await JsonManager.SavePlayerToJson(playerName);
			Dictionary<string, float> changedResources = new Dictionary<string, float>();
			changedResources.Add("honey", -honeyToEat);
			JsonManager.CreateLog(new Log
			{
				Comment = $"Complex deplicted {honeyToEat} energohoney",
				PlayerName = playerName,
				ShopName = null,
				ResourcesChanged = changedResources
			});
		}
		Debug.Log("Съели мёда: " + honeyToEat);
		ChangeHoney(-honeyToEat);
		uiResourceShower.UpdateIndicators();
	}

	private IEnumerator ConstantSeasonChanger()
	{
		while (true)
		{
			uiResourceShower.UpdateBarsStatuses();
			seasonTimeLeft = 30f;
			while (seasonTimeLeft > 0)
			{
				seasonTimeLeft -= Time.deltaTime;
				yield return null;
			}
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
					StartCoroutine(DamageRoomsBySeason());
					break;
			}
		}
	}

	public IEnumerator DamageRoomsBySeason()
	{
		for (int i = 0; i < 5; i++)
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
			Camera.main.GetComponent<CameraShake>().MeteorImpact();
			yield return new WaitForSeconds(6f);
		}
	}

	public void AddBearToMove(BearStatusController bear)
	{
		bearsToMoveOn.Add(bear);
	}

	public void SetIsGraphUsing(bool set)
	{
		isGraphUsing = set;
	}

	public float GetSeasonTimeLeft()
	{
		return seasonTimeLeft;
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