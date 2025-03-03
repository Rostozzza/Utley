using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using Random = UnityEngine.Random;

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
	public List<GameObject> bearsToSpawn = new List<GameObject>();
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
	[SerializeField] public List<BearStatusController> bearsToMoveOn;
	[SerializeField] private bool isGraphUsing = false;
	[SerializeField] private float timeLeft = ValuesHolder.GameDuration; // doesn't work, go to Awake and change;
	[SerializeField] private float seasonTimeLeft;
	[SerializeField] private float temperature = ValuesHolder.MaxTemperature;
	[SerializeField] private float timePast = 0f;
	[SerializeField] private bool isGameRunning = true;
	[SerializeField] private bool isTimeGo = true;
	[SerializeField] private bool isEnergohoneyConsuming = true;
	[SerializeField] private bool isSeasonChanging = true;
	[SerializeField] private bool isCursorAtUIDontScroll;
    [SerializeField] private int amountIsCursorAtUI;
	private bool wasSelectedThisFrame = false;
	[Header("Building settings")]
	[SerializeField] private GameObject buildingLoading;
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
	[SerializeField] public float honey;
	[SerializeField] public int asteriy;
	[SerializeField] public float astroluminite;
	[SerializeField] public float ursowaks;
	[SerializeField] public float prototype;
	[SerializeField] public float HNY;
	[SerializeField] private int rawAsterium = 0;
	[SerializeField] public int playerBears;
	public int maxBearsAmount;
	RaycastHit raycastHit;
	[Header("Effects")]
	[SerializeField] private GameObject buildingParticle;
	private bool isCold = false;
	private bool isFreezing = false;
	private int predictedAsteriumViews;

    public void SetThisFrameSelected(bool value)
	{
		wasSelectedThisFrame = value;
	}

	public void EnpowerAllRooms()
	{
		allRooms.Where(x => x.GetComponent<SupplyRoom>() && x.GetComponent<SupplyRoom>().durability > 0).ToList().ForEach(y => y.GetComponent<SupplyRoom>().GetRoomsToEnpower());
	}

	public void DecreaseTime()
	{
		timeLeft -= 30f;
		Debug.Log("Decreased time!");
	}

	public void BoostTemperature()
	{
		temperature = ValuesHolder.MaxTemperature;
		Debug.Log("Boosted temperature!");
	}

	public async Task SpawnNewBear(int amount)
	{
		await ChangeBears(amount, new Log
		{
			comment = $"Player {playerName} got new employee",
			player_name = playerName,
			resources_changed = new Dictionary<string, float> { { "player_bears_added", 1 } }
		});
		if (amount > 1)
		{
			var newBear = Instantiate(bearsToSpawn[0]);
			bears.Add(newBear);
			var newBear2 = Instantiate(bearsToSpawn[1]);
			bears.Add(newBear2);
		}
		else
		{
			var newBear = Instantiate(bearsToSpawn[playerBears - 5]);
			bears.Add(newBear);
		}
	}

	public void KillAllBuildings()
	{
		foreach (var room in allRooms)
		{
			Destroy(room);
		}
		//bearStatusListController.ClearList();
		roomStatusListController.ClearList();
		workStations.Clear();
		maxBearsAmount = 0;
		allRooms.Clear();
		allRooms = new List<GameObject>();
		asteriumRooms.Clear();
		//Destroy(asteriumRoomView[0].gameObject);
		asteriumRoomView.Clear();
		asteriumRoomView = new List<Image>();
		builderRooms.Clear();
	}

	public void LoadAllBears()
	{
		for (int i = 0; i < playerBears - 4; i++)
		{
			var newBear = Instantiate(bearsToSpawn[i]);
			bears.Add(newBear);
		}
		Debug.Log("Starting assignation of elevators..");
		foreach (var bear in bears)
		{
			bear.GetComponent<UnitMovement>().currentElevator = allRooms.First(x => x.GetComponent<Elevator>()).GetComponent<Elevator>();
			Debug.Log("Elevator assigned!(or maybe not..)");
		}
	}

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
			Debug.Log(room.Durability);
			newRoom.GetComponent<RoomScript>().durability = room.Durability;
			newRoom.GetComponent<RoomScript>().level = room.Level;
			if (newRoom.GetComponent<RoomScript>().resource == RoomScript.Resources.Cosmodrome)
			{
				ui = newRoom.GetComponentsInChildren<RectTransform>(true).First(x => x.CompareTag("cosmodromeCanvas")).gameObject;
				asteriumViewGrid = ui.GetComponentInChildren<GridLayoutGroup>(true).transform;
			}
			if (newRoom.GetComponent<RoomScript>().resource == RoomScript.Resources.Asteriy)
			{
				asteriumRooms.Add(newRoom.GetComponent<RoomScript>());
				if (allRooms.FirstOrDefault(x => x.GetComponent<RoomScript>().resource == RoomScript.Resources.Cosmodrome))
				{
					var newAsteriumView = Instantiate(asteriumViewPrefab, asteriumViewGrid);
					asteriumRoomView.Add(newAsteriumView.GetComponentInChildren<Image>(true));
					Debug.Log($"Addend new asterium! asteriumView amount: {asteriumRoomView.Count}");
				}
				else
				{
					predictedAsteriumViews++;
					Debug.Log("Queueing 1 asterium view");
				}
			}
			allRooms.Add(newRoom);
			//if (newRoom.GetComponent<SupplyRoom>())
			//{
			//	newRoom.GetComponent<SupplyRoom>().ChangeDurability(0);
			//}
			return true;
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
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
		for (int i = 0; i < predictedAsteriumViews; i++)
		{
			var newAsteriumView = Instantiate(asteriumViewPrefab, asteriumViewGrid);
			asteriumRoomView.Add(newAsteriumView.GetComponentInChildren<Image>(true));
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
		timeLeft = ValuesHolder.GameDuration;
		skyBG = GameObject.FindGameObjectWithTag("skyBG");
		StartCoroutine(ConstantDurabilityDamager((int)ValuesHolder.DurationLoss));
		StartCoroutine(ConstantEnergohoneyConsumer());
		StartCoroutine(ConstantSeasonChanger());
		StartCoroutine(ConstantTemperatureController());
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
			//selectedRoom.ToggleRoomStats(false);
			//selectedRoom.ToggleBuildStats(false);
			selectedRoom.GetRoomStatsController().SetStatsScreenShow(false);
		}
		selectedRoom = null;
		foreach (GameObject room in allRooms)
		{
			foreach (var button in room.GetComponentsInChildren<Button>(true).Where(x => !x.CompareTag("assignment") && !x.CompareTag("dont_hide_button")).ToList())
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
		//Image infoButton = GameObject.FindGameObjectWithTag("info_button").GetComponent<Image>(); //disabled
		if (this.mode == (Mode)mode)
		{
			switch (this.mode)
			{
				case Mode.Build:
					buildButton.sprite = defaultBuildButton;
					break;
				case Mode.Info:
					//infoButton.sprite = defaultInfoButton;
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
					//infoButton.sprite = defaultInfoButton;
					break;
			}
			switch ((Mode)mode)
			{
				case Mode.Build:
					buildButton.sprite = selectedBuildButton;
					break;
				case Mode.Info:
					//infoButton.sprite = selectedInfoButton;
					break;
			}
			SetMode((Mode)mode);
		}
	}

	/// <summary>
	/// Builds a room from variable "building" at position of "queuedBuildPosition"
	/// </summary>
	/// <param name="building"></param>
	public void SelectAndBuild(GameObject building)
	{
		if (isAPIActive)
		{
			buildingLoading.SetActive(true);
		}
		SelectAndBuildAsync(building);
	}

	public async Task SelectAndBuildAsync(GameObject building)
	{
		var currentAsterium = await GetAsteriy();
		if (queuedBuildPositon.GetComponentInParent<Elevator>() == null) queuedBuildPositon.GetComponent<Button>().interactable = false;
		if (!building.CompareTag("elevator"))
		{
			RoomScript roomScript = building.GetComponent<RoomScript>();
			var currentHoney = await GetHoney();
			var currentAstroluminite = await GetAstroluminite();
			if (currentAsterium < roomScript.asteriumCost || currentHoney < roomScript.honeyCost || currentAstroluminite < roomScript.astroluminiteCost)
			{
				queuedBuildPositon.GetComponentInChildren<Button>(true).interactable = true;
				queuedBuildPositon = null;
				buildingScreen.SetActive(false);
				elevatorBuildingScreen.SetActive(false);
				EventManager.callError.Invoke($"Недостаточно {(currentAsterium < roomScript.asteriumCost ? "<color=yellow>" + (Mathf.Abs(currentAsterium - roomScript.asteriumCost)) + "</color>" + " астериума;" : "")}" +
					$"{(currentHoney < roomScript.honeyCost ? "<color=yellow>" + ((int)Mathf.Abs(currentHoney - roomScript.honeyCost)) + "</color>" + " энергомёда;" : "")}" +
					$"{(currentAstroluminite < roomScript.astroluminiteCost ? "<color=yellow>" + (Mathf.Abs(currentAstroluminite - roomScript.astroluminiteCost)) + "</color>" + " астролюминита;" : "")}");
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
			if (fixedBuilderRoom == null || !fixedBuilderRoom.GetComponent<BuilderRoom>().GetWait())
			{
				Debug.Log("Нет свободных строительных комплексов!");
				EventManager.callWarning.Invoke($"Назначьте персонажа на комплекс строительства.");
				queuedBuildPositon.GetComponentInChildren<Button>(true).interactable = true;
				queuedBuildPositon = null;
				buildingScreen.SetActive(false);
				elevatorBuildingScreen.SetActive(false);
				return;
			}

			if (roomScript.asteriumCost > 0)
			{
				await ChangeAsteriy(-roomScript.asteriumCost, new Log
				{
					comment = $"Consumed {Mathf.Abs(roomScript.asteriumCost)} asterium from player {playerName} for building {roomScript.gameObject.name}",
					player_name = playerName,

					resources_changed = new Dictionary<string, float> { { "asterium", -roomScript.asteriumCost } }
				});
			}
			if (roomScript.honeyCost > 0)
			{
				await ChangeHoney(-roomScript.honeyCost, new Log
				{
					comment = $"Consumed {Mathf.Abs(roomScript.honeyCost)} honey from player {playerName} for building {roomScript.gameObject.name}",
					player_name = playerName,

					resources_changed = new Dictionary<string, float> { { "honey", -roomScript.honeyCost } }
				});
			}
			if (roomScript.astroluminiteCost > 0)
			{
				await ChangeAstroluminite(-roomScript.astroluminiteCost, new Log
				{
					comment = $"Consumed {Mathf.Abs(roomScript.astroluminiteCost)} astroluminite from player {playerName} for building {roomScript.gameObject.name}",
					player_name = playerName,

					resources_changed = new Dictionary<string, float> { { "astroluminite", -roomScript.astroluminiteCost } }
				});
			}
			uiResourceShower.UpdateIndicators();
		}
		else
		{
			if (currentAsterium < 10)
			{
				EventManager.callWarning.Invoke($"Не хватает {10 - currentAsterium} астерия.");
				queuedBuildPositon = null;
				buildingScreen.SetActive(false);
				elevatorBuildingScreen.SetActive(false);
				queuedBuildPositon.GetComponentInChildren<Button>(true).interactable = true;
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
				EventManager.callWarning.Invoke($"Назначьте персонажа на комплекс строительства.");
				queuedBuildPositon = null;
				buildingScreen.SetActive(false);
				elevatorBuildingScreen.SetActive(false);
				return;
			}
			await ChangeAsteriy(-10, new Log
			{
				comment = $"Consumed 10 asterium from player {playerName} for building an elevator",
				player_name = playerName,

				resources_changed = new Dictionary<string, float> { { "asterium", -10 } }
			});
		}
		if (isAPIActive)
		{
			buildingLoading.SetActive(false);
		}
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
		Debug.Log("NoT DEAD");
		buildingScreen.SetActive(false);
		elevatorBuildingScreen.SetActive(false);
	}

	private IEnumerator SelectAndBuildWaiter(GameObject building, GameObject room, Transform point)
	{
		room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		while (room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoutine != null)
		{
			yield return null;
		}
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", true);
		Instantiate(buildingParticle, point.position, Quaternion.identity);
		if (!building.CompareTag("elevator"))
		{
			if (point.position.x < point.parent.parent.position.x)
			{
				Instantiate(buildingParticle, point.position + Vector3.left * 4f, Quaternion.identity);
			}
			else
			{
				Instantiate(buildingParticle, point.position + Vector3.right * 4f, Quaternion.identity);
			}
		}
		room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(true);
		yield return new WaitForSeconds(5);
		room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().LevelUpBear();
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", false);
		room.GetComponent<RoomScript>().SetStatus(RoomScript.Status.Free);
		SelectAndBuildMainBlock(building, point).Wait();
		Debug.Log("NoT DEAD after building");
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
				//supply.InitializeGraph(); // outdated, should be commented if we using exercise in tablet 🤬;
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
		if (point.parent.parent.tag == "elevator" && point.parent.parent.position.y != instance.transform.position.y && instance.tag == "elevator")
		{
			queuedBuildPositon = null;
			return;
		}
		queuedBuildPositon = null;
		allRooms.Add(instance);
		if (!instance.TryGetComponent<SupplyRoom>(out SupplyRoom s))
		{
			allRooms.Where(x => x.GetComponent<SupplyRoom>()).ToList().ForEach(y => y.GetComponent<SupplyRoom>().GetRoomsToEnpower());
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
			honey = float.Parse(model.resources["honey"].Replace('.', ','));
			return float.Parse(model.resources["honey"].Replace('.', ','));
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
			this.asteriy = int.Parse(model.resources["asterium"]);
			return int.Parse(model.resources["asterium"]);
		}
		return asteriy;
	}

	public async Task<float> GetAstroluminite()
	{
		if (isAPIActive)
		{
			var model = await RequestManager.GetPlayer(playerName);
			playerModel = model;
			astroluminite = float.Parse(model.resources["astroluminite"].Replace('.', ','));
			return float.Parse(model.resources["astroluminite"].Replace('.', ','));
		}
		return astroluminite;
	}

	public async Task<float> GetUrsowaks()
	{
		if (isAPIActive)
		{
			var model = await RequestManager.GetPlayer(playerName);
			playerModel = model;
			ursowaks = float.Parse(model.resources["ursowaks"].Replace('.', ','));
			return float.Parse(model.resources["ursowaks"].Replace('.', ','));
		}
		return ursowaks;
	}

	public async Task<float> GetPrototype()
	{
		if (isAPIActive)
		{
			var model = await RequestManager.GetPlayer(playerName);
			playerModel = model;
			prototype = float.Parse(model.resources["prototype"].Replace('.', ','));
			return float.Parse(model.resources["prototype"].Replace('.', ','));
		}
		return prototype;
	}

	public async Task<float> GetHNY()
	{
		if (isAPIActive)
		{
			var model = await RequestManager.GetPlayer(playerName);
			playerModel = model;
			HNY = float.Parse(model.resources["HNY"].Replace('.', ','));
			return float.Parse(model.resources["HNY"].Replace('.', ','));
		}
		return HNY;
	}

	public async Task<int> GetBears()
	{
		if (isAPIActive)
		{
			var model = await RequestManager.GetPlayer(playerName);
			playerModel = model;
			playerBears = int.Parse(model.resources["bears"].Replace('.', ','));
			return int.Parse(model.resources["bears"].Replace('.', ','));
		}
		return playerBears;
	}

	/// <summary>
	/// Changes amount of honey by given number
	/// </summary>
	/// <param name="amount"></param>
	public async Task ChangeHoney(float amount, Log log)
	{
		if (isAPIActive && (honey + amount < Mathf.Floor(honey) || amount > 0))
		{
			float serverHoney = await GetHoney();
			serverHoney += amount;
			serverHoney = Mathf.Clamp(serverHoney, 0, 999);
			honey = serverHoney;
			await JsonManager.SavePlayerToJson(playerName);
			await JsonManager.CreateLog(log);
			return;
		}
		honey += amount;
		honey = Mathf.Clamp(honey, 0, 999);
	}

	/// <summary>
	/// Changes amount of asterium by given number
	/// </summary>
	/// <param name="amount"></param>
	public async Task ChangeAsteriy(int amount, Log log)
	{
		if (isAPIActive)
		{
			int serverAsterium = await GetAsteriy();
			serverAsterium += amount;
			serverAsterium = Mathf.Clamp(serverAsterium, 0, 999);
			asteriy = serverAsterium;
			await JsonManager.SavePlayerToJson(playerName);
			await JsonManager.CreateLog(log);
			return;
		}
		asteriy += amount;
		asteriy = Mathf.Clamp(asteriy, 0, 999);
	}

	public async Task ChangeAstroluminite(float amount, Log log)
	{
		if (isAPIActive)
		{
			float serverAstroluminite = await GetAstroluminite();
			serverAstroluminite += amount;
			serverAstroluminite = Mathf.Clamp(serverAstroluminite, 0, 999);
			astroluminite = serverAstroluminite;
			await JsonManager.SavePlayerToJson(playerName);
			await JsonManager.CreateLog(log);
			return;
		}
		astroluminite += amount;
		astroluminite = Mathf.Clamp(astroluminite, 0, 999);
	}

	public async Task ChangeUrsowaks(float amount, Log log)
	{
		if (isAPIActive)
		{
			float serverUrsowaks = await GetUrsowaks();
			serverUrsowaks += amount;
			serverUrsowaks = Mathf.Clamp(serverUrsowaks, 0, 999);
			ursowaks = serverUrsowaks;
			await JsonManager.SavePlayerToJson(playerName);
			await JsonManager.CreateLog(log);
			return;
		}
		ursowaks += amount;
		ursowaks = Mathf.Clamp(ursowaks, 0, 999);
	}

	public async Task ChangePrototype(float amount, Log log)
	{
		if (isAPIActive)
		{
			float serverPrototype = await GetPrototype();
			serverPrototype += amount;
			serverPrototype = Mathf.Clamp(serverPrototype, 0, 999);
			prototype = serverPrototype;
			await JsonManager.SavePlayerToJson(playerName);
			await JsonManager.CreateLog(log);
			return;
		}
		prototype += amount;
		prototype = Mathf.Clamp(prototype, 0, 999);
	}

	public async Task ChangeHNY(float amount, Log log)
	{
		if (isAPIActive)
		{
			float serverHNY = await GetHNY();
			serverHNY += amount;
			serverHNY = Mathf.Clamp(serverHNY, -999, 999);
			HNY = serverHNY;
			await JsonManager.SavePlayerToJson(playerName);
			await JsonManager.CreateLog(log);
			return;
		}
		HNY += amount;
		HNY = Mathf.Clamp(HNY, -999, 999);
	}

	public async Task ChangeBears(int amount, Log log)
	{
		if (isAPIActive)
		{
			int serverBears = await GetBears();
			serverBears += amount;
			serverBears = Mathf.Clamp(serverBears, -999, 999);
			playerBears = serverBears;
			await JsonManager.SavePlayerToJson(playerName);
			await JsonManager.CreateLog(log);
			return;
		}
		playerBears += amount;
		playerBears = Mathf.Clamp(playerBears, 0, 6);
		Debug.Log("Changed bears!");
	}

	public bool FlyForRawAsterium() => rawAsterium < asteriumRooms.Where(x => x.GetComponent<RoomScript>().isEnpowered && !x.GetComponent<RoomScript>().isReadyForWork).ToList().Count;

	public void DeliverRawAsterium()
	{
		rawAsterium++;
		TryProcessingRawAsterium();
	}

	public void TryProcessingRawAsterium()
	{
		var targetedRoom = asteriumRooms.FirstOrDefault(x => !x.isReadyForWork && x.CheckIfSolved() && x.isEnpowered);
		if (targetedRoom == null || rawAsterium <= 0)
		{
			return;
		}
		targetedRoom.isReadyForWork = true;
		targetedRoom.StartWork(gameObject);
		rawAsterium--;
		var targetedView = asteriumRoomView.FirstOrDefault(x => x.color != (Color.red + Color.yellow) / 2f);
		if (targetedView == null)
		{
			return;
		}
		targetedView.color = (Color.red + Color.yellow) / 2f;
	}

	public void WithdrawRawAsterium()
	{
		var targetedView = asteriumRoomView.FirstOrDefault(x => x.color == (Color.red + Color.yellow) / 2f);
		if (targetedView == null)
		{
			return;
		}
		targetedView.color = Color.grey;

	}

	private void Update()
	{
		InputHandler();
		TimeGoHandler();
	}

	private void TimeGoHandler()
	{
		if (isTimeGo)
		{
			if (timeLeft <= 0 && isGameRunning)
			{
				MenuManager.Instance.ShowWinScreen();
			}
			timeLeft -= Time.deltaTime;
			timePast += Time.deltaTime;
		}
	}

	/// <summary>
	/// true - time goes, false - time stops. Time like "resource" at top bar.
	/// </summary>
	/// <param name="set"></param>
	public void SetTimeGo(bool set)
	{
		isTimeGo = set;
	}

	private void InputHandler()
	{
		if (Input.GetMouseButtonUp(0)) //by some reason InputController bugs here;
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
					//if (amountIsCursorAtUI > 0) return;
					//OutlineWorkStations(false);
					bears.ForEach(x => x.GetComponent<UnitScript>().SetMarker(false));
					HideAllAssignButtons();
				}
			}
			else
			{
				//OutlineWorkStations(false);
				if (!isCursorAtUIDontScroll) selectedUnit = null;
				if (selectedRoom != null)
				{
					selectedRoom.ToggleRoomStats(false);
					selectedRoom.ToggleBuildStats(false);
				}
				selectedRoom = null;
				if (!isCursorAtUIDontScroll) selectedUnit = null;
			}
		}
		else if (InputController.GetKeyDown(ActionKeys.MoveBearToRoom)) //off
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out raycastHit, 100f) && !buildingScreen.activeSelf)
			{
				if (raycastHit.transform != null)
				{
					RightClick(raycastHit.transform.gameObject);
					//OutlineWorkStations(false);
				}
			}
		}
		else if (InputController.GetKeyDown(ActionKeys.Quit) && (buildingScreen.activeSelf || elevatorBuildingScreen.activeSelf))
		{
			buildingScreen.SetActive(false);
			elevatorBuildingScreen.SetActive(false);
		}

		if (!isGraphUsing)
		{
			if (InputController.GetKeyDown(ActionKeys.Bear1) && bearsToMoveOn.Count > 0)
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
			else if (InputController.GetKeyDown(ActionKeys.Bear2) && bearsToMoveOn.Count > 1)
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
			else if (InputController.GetKeyDown(ActionKeys.Bear3) && bearsToMoveOn.Count > 2)
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
			else if (InputController.GetKeyDown(ActionKeys.Bear4) && bearsToMoveOn.Count > 3)
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
			else if (InputController.GetKeyDown(ActionKeys.Bear5) && bearsToMoveOn.Count > 4)
			{
				if (selectedUnit != bearsToMoveOn[4].GetBearObj())
				{
					bearsToMoveOn[4].MoveToObject();
				}
				else
				{
					selectedUnit.GetComponent<UnitScript>().SetMarker(false);
					selectedUnit.GetComponent<UnitScript>().GetStatusPanel().SetSelect(false);
					HideAllAssignButtons();
					selectedUnit = null;
				}
			}
			else if (InputController.GetKeyDown(ActionKeys.Bear6) && bearsToMoveOn.Count > 5)
			{
				if (selectedUnit != bearsToMoveOn[5].GetBearObj())
				{
					bearsToMoveOn[5].MoveToObject();
				}
				else
				{
					selectedUnit.GetComponent<UnitScript>().SetMarker(false);
					selectedUnit.GetComponent<UnitScript>().GetStatusPanel().SetSelect(false);
					HideAllAssignButtons();
					selectedUnit = null;
				}
			}
		}

		if (InputController.GetKeyDown(ActionKeys.BuildMode)) SetModeByButton((int)Mode.Build);
		//else if (Input.GetKeyDown(KeyCode.I)) SetModeByButton((int)Mode.Info);
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
		//if (selectedUnit == null)
		//{
		//	Debug.Log("Не выбран юнит");
		//	return;
		//}
		//if (gameObject.CompareTag("room") && !selectedUnit.GetComponent<UnitMovement>().IsWalkingToWork())
		//{
		//	//selectedUnit.GetComponent<UnitMovement>().StopAllCoroutines();
		//	//selectedUnit.GetComponent<UnitMovement>().MoveToRoom(gameObject.GetComponent<RoomScript>());
		//	//if (builderRooms.Any(x => x.GetComponent<BuilderRoom>().fixedBear == selectedUnit) && builderRooms.Where(x => x.GetComponent<BuilderRoom>().fixedBear == selectedUnit).ToList()[0].GetComponent<BuilderRoom>().GetWait())
		//	//{
		//	//	if (builderRooms.Any(x => x.GetComponent<BuilderRoom>().fixedBear == selectedUnit))
		//	//	{
		//	//		builderRooms.Where(x => x.GetComponent<BuilderRoom>().fixedBear == selectedUnit).ToList()[0].GetComponent<BuilderRoom>().SetWait(false, true);
		//	//		//builderRooms.Where(x => x.GetComponent<BuilderRoom>().fixedBear == unit).ToList()[0].GetComponent<BuilderRoom>().InterruptWork();
		//	//	}
		//	//}
		//}
		if (gameObject.TryGetComponent(out RoomScript roomScript))
		{
			bool shouldWeShowStats = !roomScript.GetRoomStatsController().GetIsStatsActive();
			if (shouldWeShowStats)
			{
				foreach (var room in allRooms.ConvertAll(r => r.GetComponent<RoomScript>()))
				{
					try 
					{
						room.GetRoomStatsController().SetStatsScreenShow(false);
					} catch {}
				}
			}
			roomScript.GetRoomStatsController().SetStatsScreenShow(shouldWeShowStats);
		}
		if (selectedUnit)
		{
			selectedUnit.GetComponent<UnitScript>().SetMarker(false);
			selectedUnit.GetComponent<UnitScript>().GetStatusPanel().SetSelect(false);
			selectedUnit = null;
		}
		HideAllAssignButtons();
	}

	public void WalkAndWork(GameObject unit, GameObject obj)
	{
		StartCoroutine(WalkAndStartWork(unit, obj));
	}

	private IEnumerator WalkAndStartWork(GameObject unit, GameObject obj) // needs to wait for walk and after we starting work
	{
		bool builderGoesBack = false;
		//Debug.Log(unit + "|" + obj);
		if (!unit.GetComponent<UnitMovement>().IsWalkingToWork())
		{
			unit.GetComponent<UnitMovement>().StopAllCoroutines();
			unit.GetComponent<UnitMovement>().MoveToRoom(obj.GetComponentInParent<RoomScript>());
			unit.GetComponent<UnitMovement>().SetIsWalkingToWork(true);
			if (selectedUnit) selectedUnit.GetComponent<UnitScript>().SetMarker(false);
			var enRouteButton = obj.GetComponentsInChildren<ButtonEnRoute>(true)[0];
			if (obj.GetComponentInParent<RoomScript>().resource == RoomScript.Resources.Build)
			{
				if (obj.GetComponentInParent<BuilderRoom>().fixedBear != null)
				{
					enRouteButton.SetButtonState(true);
					builderGoesBack = true;
				}
				else
				{
					enRouteButton.SetButtonState(false);
				}
			}
			else
			{
				enRouteButton.SetButtonState(false);
			}
			selectedUnit = null;
			while (unit.GetComponent<UnitMovement>().currentRoutine != null)
			{
				if ((builderGoesBack) ? !enRouteButton.IsButtonPressed() : enRouteButton.IsButtonPressed())//(obj.GetComponentInParent<RoomScript>().resource == RoomScript.Resources.Build) ? obj.GetComponentInParent<BuilderRoom>().fixedBear != null && !enRouteButton.IsButtonPressed() : enRouteButton.IsButtonPressed())
				{
					Debug.Log("Отменили");
					unit.GetComponent<UnitMovement>().StopAllCoroutines();
					unit.GetComponent<UnitMovement>().SetIsWalkingToWork(false);
					unit.GetComponent<UnitMovement>().MoveToRoom(unit.GetComponent<UnitMovement>().currentRoom);
					break;
				}
				yield return null;
			}
			//bool builderRoomCondition = (obj.GetComponentInParent<RoomScript>().resource == RoomScript.Resources.Build) && (obj.GetComponentInParent<BuilderRoom>().fixedBear != null) && unit.GetComponent<UnitMovement>().IsWalkingToWork();
			if ((!enRouteButton.IsButtonPressed() && unit.GetComponent<UnitMovement>().IsWalkingToWork()) || builderGoesBack)//(builderGoesBack) ? unit.GetComponent<UnitMovement>().IsWalkingToWork() : !enRouteButton.IsButtonPressed() && unit.GetComponent<UnitMovement>().IsWalkingToWork())// || builderRoomCondition)
			{
				if (obj.GetComponentInParent<RoomScript>().resource == RoomScript.Resources.Build) obj.GetComponentInParent<BuilderRoom>().SetWait(true);
				obj.GetComponentInParent<RoomScript>().StartWork(unit);
				enRouteButton.SetButtonState(true);
				unit.GetComponent<UnitScript>().SetMarker(false);
				OutlineWorkStations(false);
				unit.GetComponent<UnitMovement>().SetIsWalkingToWork(false);
			}
		}
	}

	public void HideAllAssignButtons()
	{
		//.ForEach(y => y.GetComponent<RoomScript>().HideButton());
		foreach (var room in allRooms.Where(x => x.GetComponent<RoomScript>()).ToList())
		{
			if (!room.CompareTag("dont_hide_button"))
			{
				room.GetComponent<RoomScript>().HideButton();
			}
		}
	}

	public void ShowAvailableAssignments()
	{
		var interestingRooms = allRooms.Where(x => x.GetComponent<RoomScript>() && (x.GetComponentInChildren<ButtonEnRoute>(true) && !x.GetComponentInChildren<ButtonEnRoute>(true).GetComponent<Button>().interactable)).ToList();
		//interestingRooms.ForEach(x => Debug.Log("<color=\"green\">" + x + "</color>"));
		foreach (var room in interestingRooms)
		{
			if (room.TryGetComponent<BuilderRoom>(out BuilderRoom builder))
			{
				if (builder.fixedBear == null) // Shows assign button only if it's hasn't fixedBear
				{
					room.GetComponent<RoomScript>().ShowButton();
				}
			}
			else
			{
				room.GetComponent<RoomScript>().ShowButton();
			}
		}
	}

	public void ClickedGameObject(GameObject gameObject)
	{
		Debug.Log("кликнули по <color=\"yellow\">" + gameObject.tag + "</color>");
		if (gameObject.CompareTag("unit"))
		{
			if (gameObject.GetComponent<UnitScript>().selectable)
			{
				bears.ForEach(x => x.GetComponent<UnitScript>().SetMarker(false));
				switch (mode)
				{
					case Mode.None:
						Debug.Log("Select");
						gameObject.GetComponent<UnitScript>().SelectUnit();
						break;
					case Mode.Info:
						//SetModeByButton((int)Mode.None);
						//gameObject.GetComponent<UnitScript>().SelectUnit();
						//gameObject.GetComponent<UnitScript>().SetStatsScreen();
						break;
					case Mode.Build:
						SetModeByButton((int)Mode.None);
						GetComponent<UnitScript>().SelectUnit();
						break;
				}
			}
		}
		else
		{
			if (wasSelectedThisFrame)
			{
				Debug.Log("bonk");
				return;
			}
			bears.ForEach(x => x.GetComponent<UnitScript>().SetMarker(false));
			if (selectedUnit != null)
			{
				selectedUnit.GetComponent<UnitScript>().SetMarker(false);
				selectedUnit.GetComponent<UnitScript>().GetStatusPanel().SetSelect(false);
				HideAllAssignButtons();
				selectedUnit = null;
			}
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
		while (true && isTimeGo)
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
			while (isEnergohoneyConsuming)
			{
				yield return new WaitForSeconds(1);
				ConsumeEnergohoney();
			}
			yield return null;
		}
	}

	public void SetEnergohoneyConsume(bool set)
	{
		isEnergohoneyConsuming = set;
	}

	private async Task ConsumeEnergohoney()
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
			//var model = await JsonManager.SavePlayerToJson(playerName);
			Dictionary<string, float> changedResources = new Dictionary<string, float>();
			changedResources.Add("honey", -honeyToEat);
			//JsonManager.CreateLog(new Log
			//{
			//	Comment = $"Complex deplicted {honeyToEat} energohoney",
			//	PlayerName = playerName,
			//	ShopName = null,
			//	ResourcesChanged = changedResources
			//});
		}
		await ChangeHoney(-honeyToEat, new Log
		{
			comment = $"Deplicted honey for maintaining facility temperature of player {playerName}",
			resources_changed = new Dictionary<string, float> { { "player_honey", honeyToEat } },
			player_name = playerName
		});
		//Debug.Log("Съели мёда: " + honeyToEat);
		uiResourceShower.UpdateIndicators();
	}

	private IEnumerator ConstantSeasonChanger()
	{
		while (true)
		{
			while (isSeasonChanging)
			{
				uiResourceShower.UpdateBarsStatuses();
				seasonTimeLeft = 30f;
				while (seasonTimeLeft > 0)
				{
					seasonTimeLeft -= Time.deltaTime;
					yield return null;
				}
				if (!isSeasonChanging) break;
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
			yield return null;
		}
	}

	public void SetSeasonChange(bool set)
	{
		isSeasonChanging = set;
	}

	public IEnumerator DamageRoomsBySeason()
	{
		if (isTimeGo)
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
					float damage = (0.35f / 5f + 0.02f * cycleNumber - 0.02f * room.GetComponent<RoomScript>().depthLevel) / 2;
					Debug.Log(room.name + " задамажен фазой на " + damage);
					room.GetComponent<RoomScript>().ChangeDurability(-damage);
				});
				if (ShopManager.Instance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("TabletHide")) Camera.main.GetComponent<CameraShake>().MeteorImpact();
				yield return new WaitForSeconds(6f);
			}
		}
	}

	private IEnumerator ConstantTemperatureController()
	{
		while (true)
		{
			if (honey <= 0)
			{
				if (season == Season.Freeze)
				{
					temperature -= 0.5f * 1.25f * Time.deltaTime;
				}
				else
				{
					temperature -= 0.5f * Time.deltaTime;
				}
				if (!isCold && temperature <= 0f)
				{
					isCold = true;
					bears.ForEach(x => x.GetComponent<UnitScript>().StartBreath());
				}
				if (!isFreezing && temperature <= -10f)
				{
					isFreezing = true;
					bears.ForEach(x => x.GetComponent<UnitScript>().StartFreezing());
				}
			}
			else
			{
				temperature += 10f / 30f * Time.deltaTime;
				if (isCold && temperature >= 0f)
				{
					isCold = false;
					bears.ForEach(x => x.GetComponent<UnitScript>().StopBreath());
				}
				if (isFreezing && temperature >= -10f)
				{
					isFreezing = false;
					bears.ForEach(x => x.GetComponent<UnitScript>().StopFreezing());
				}
			}
			if (temperature <= -ValuesHolder.MinTemperature && isGameRunning)
			{
				Debug.Log("ПОРАЖЕНИЕ");
				MenuManager.Instance.ShowLoseScreen();
			}
			temperature = Mathf.Clamp(temperature, -ValuesHolder.MinTemperature, ValuesHolder.MaxTemperature);
			yield return null;
		}
	}

	public void ActivateTutorialMode()
	{

	}

	public void CallShopFromGameManager()
	{
		MenuManager.Instance.shopScreen.SetActive(true);
		ShopManager.Instance.OpenShop();
	}

	public void AddBearToMove(BearStatusController bear)
	{
		bearsToMoveOn.Add(bear);
	}

	public void SetIsGraphUsing(bool set)
	{
		isGraphUsing = set;
	}

	public bool GetIsGraphUsing()
	{
		return isGraphUsing;
	}

	public float GetSeasonTimeLeft()
	{
		return seasonTimeLeft;
	}

	public float GetTimeLeft()
	{
		return timeLeft;
	}

	public float GetTemperature()
	{
		return temperature;
	}

	public float GetTimePast()
	{
		return timePast;
	}

	public void SetIsGameRunning(bool set)
	{
		isGameRunning = set;
	}

	public void SetIsCursorAtUIDontScroll(bool set)
	{
		isCursorAtUIDontScroll = set;
	}

	public bool GetIsCursorAtUIDontScroll()
	{
		return isCursorAtUIDontScroll;
	}

	public void SetIsCursorAtUI(bool set)
	{
		amountIsCursorAtUI += set ? 1 : -1;
	}

	public int GetIsCursorAtUI() => amountIsCursorAtUI;

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

//__/\\\________/\\\________________/\\\\\\_________________________________        
// _\/\\\_______\/\\\_______________\////\\\_________________________________       
//  _\/\\\_______\/\\\_____/\\\_________\/\\\______________________/\\\__/\\\_      
//   _\/\\\_______\/\\\__/\\\\\\\\\\\____\/\\\________/\\\\\\\\____\//\\\/\\\__     
//    _\/\\\_______\/\\\_\////\\\////_____\/\\\______/\\\/////\\\____\//\\\\\___    
//     _\/\\\_______\/\\\____\/\\\_________\/\\\_____/\\\\\\\\\\\______\//\\\____   
//      _\//\\\______/\\\_____\/\\\_/\\_____\/\\\____\//\\///////____/\\_/\\\_____  
//       __\///\\\\\\\\\/______\//\\\\\____/\\\\\\\\\__\//\\\\\\\\\\_\//\\\\/______ 
//        ____\/////////_________\/////____\/////////____\//////////___\////________