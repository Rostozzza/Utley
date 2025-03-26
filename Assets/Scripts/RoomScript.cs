using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using System;

using Random = UnityEngine.Random;
using UnityEditor;
using UnityEngine.Video;

public class RoomScript : MonoBehaviour
{
	[SerializeField] public Status status;
	[SerializeField] public Resources resource;
	[SerializeField] private GameObject roomStatsScreen;
	[SerializeField] protected RoomStatsController roomStatsController;
	//[SerializeField] private GameObject roomBuildScreen;
	public Room roomModel;
	[SerializeField] public int asteriumCost;
	[SerializeField] public int honeyCost;
	[SerializeField] public int astroluminiteCost;
	[Header("Camera task settings")]
	[SerializeField] protected Transform cameraPoint;
	[SerializeField] protected Vector3 cameraAngle;

	public List<Elevator> connectedElevators;
	public List<RoomScript> connectedRooms;
	private Coroutine work;
	//[SerializeField] private Transform[] rawWalkPoints; // for energohoney 1 - 3 is paseka, 4 is generator
	[SerializeField] private List<Transform> rawWalkPoints;
	//private Vector3[] walkPoints;
	private List<Vector3> walkPoints;
	//[SerializeField] protected TextMeshProUGUI timeShow;
	[SerializeField] public GameObject fixedBear;
	[SerializeField] private List<GameObject> workStationsToOutline;
	private TextMeshProUGUI hullPercentage;
	private Transform hullBar;
	private TextMeshProUGUI levelText;
	[SerializeField] public float durability = 1f;
	[SerializeField] public int level = 1;
	[SerializeField] public int depthLevel;
	[SerializeField] private List<ParticleSystem> sparks;
	[SerializeField] private List<GameObject> lamps;
	[SerializeField] private GameObject baseOfRoom;
	private Color defaultLampColor;
	private Color defaultBaseColor;
	[SerializeField] protected Animator animator;
	public bool isEnpowered = false;
	[SerializeField] protected RoomStatusController statusPanel;
	public string workStr;
	[SerializeField] private bool waitForPermissionToContinue;
	[SerializeField] protected GameObject assignmentButton;
	[SerializeField] protected bool isSolved = false;
	Coroutine blinks = null;
	[Header("Audio Settings")]
	[SerializeField] protected AudioSource audioSource;
	[SerializeField] protected AudioClip workSound;
	[Header("Progress bar")]
	[SerializeField] private Image progressBar;
	[SerializeField] private Image upgradeBar;
	[Header("Video Work")]
	[SerializeField] protected List<VideoPlayer> videoPlayers;
	[Header("Asterium Settings")]
	[SerializeField] private FlyForType flyForType;
	[SerializeField] private GameObject cosmodromeSelectScreen;
	public bool isReadyForWork = false;
	[SerializeField] public GameObject takeALookButton;
	[SerializeField] private GameObject coneierScreen;
	[Header("Work Settings")]
	protected float SpeedByBearLevelCoef = ValuesHolder.InteractionSpeedMultiplyerByLevel;
	protected float SpeedByRoomLevelCoef = ValuesHolder.InteractionSpeedMultiplyerByGrade;
	protected float StandartInteractionTime = ValuesHolder.StandartInteractionTime;
	protected float SpeedByUsingSuitableBearCoef = ValuesHolder.InteracionTimeMultiplyerByCorrectJob;
	protected float efficientyCoeficent = 1f;
	[SerializeField] protected RoomWorkUI workUI;
	[SerializeField] private GameObject efficiencyDownPanel;
	[SerializeField] private TextMeshProUGUI efficiencyDownPercentageText;
	[SerializeField] private Slider progressbar;
	[Header("Cosmodrome Only")]
	[SerializeField] private Image buttonBGImg;
	[SerializeField] private Image circleTimer;
	[SerializeField] private Image gearImg;
	[SerializeField] private Image circleFrame;
	public void SetWorkEfficiency(float newCoef, bool isCosmodromeWait = true, bool isThisFirstCall = false) // Last parameter is KOSTYL'
	{
		var efficiencyAnim = efficiencyDownPanel.GetComponentInChildren<Animator>(true);
		switch (resource)
		{
			case Resources.Cosmodrome:
				efficiencyDownPercentageText.text = isThisFirstCall ? "" : $"-{(1 - newCoef) * 100}%";
				if (efficientyCoeficent <= 1f && newCoef >= 1f)
				{
					if (!isThisFirstCall) efficiencyAnim.SetTrigger("HidePanel");
					circleTimer.fillAmount = 1;
					StartCoroutine(CosmodromeCircleTimer(150));
				}
				else if (newCoef < 1f)
				{
					if (isCosmodromeWait)
					{
						SetConeierScreen(true);
						efficiencyAnim.SetTrigger("ShowPanel");
					}
					else
					{
						SetConeierScreen(false);
						efficiencyAnim.SetTrigger("MakeGearRed");
						StartCoroutine(WaitForWorkEfficiencyRetry(30));
					}
				}
				efficientyCoeficent = newCoef;
				break;
			default:
				efficiencyDownPercentageText.text = $"-{(1 - newCoef) * 100}%";
				if (efficientyCoeficent <= 1f && newCoef >= 1f)
				{
					efficiencyAnim.SetTrigger("HideCompletely");
				}
				if (newCoef < 1f)
				{
					efficiencyAnim.SetTrigger("ShowPanel");
					StartCoroutine(WaitForWorkEfficiencyRetry(60));
				}
				efficientyCoeficent = newCoef;
				break;
		}
	}


	private IEnumerator WaitForWorkEfficiencyRetry(int time)
	{
		if (circleFrame) circleFrame.enabled = false;
		if (progressbar) progressbar.gameObject.SetActive(true);
		float timeSpent = 0f;
		SetConeierScreen(false);
		while (timeSpent <= time)
		{
			timeSpent += Time.deltaTime;
			if (progressbar) progressbar.value = 1 - timeSpent / time;
			yield return null;
		}
		if (resource == Resources.Cosmodrome) efficiencyDownPanel.GetComponent<Animator>().SetTrigger("UnmakeGearRed");
		if (progressbar) progressbar.gameObject.SetActive(false);
		if (resource != Resources.Cosmodrome) efficiencyDownPanel.GetComponent<Animator>().SetTrigger("HidePanel");
		if (resource != Resources.Cosmodrome) efficiencyDownPercentageText.text = "";
		SetConeierScreen(true);
		ShowSetPipesButtonScreen();
	}

	private IEnumerator IncorrectAnswer(float timeToWait)
	{
		efficiencyDownPanel.GetComponent<Animator>().SetTrigger("UnmakeGearRed");
		yield return new WaitForSeconds(timeToWait);
		efficiencyDownPanel.GetComponent<Animator>().SetTrigger("HidePanel");
	}

	private IEnumerator CosmodromeCircleTimer(float time)
	{
		if (circleFrame) circleFrame.enabled = true;
		efficiencyDownPercentageText.text = "";
		SetConeierScreen(false);
		float timeSpent = 0f;
		while (timeSpent <= time)
		{
			timeSpent += Time.deltaTime;
			circleTimer.fillAmount = 1 - timeSpent / time;
			yield return null;
		}
		SetWorkEfficiency(1 - 0.3f, true);
	}

	public virtual void Enpower()
	{
		isEnpowered = true;
		ChangeDurability(0);
		SetConeierScreenShow(true);
		//Debug.Log($"Empowered roon {gameObject.name}");
	}

	public virtual void Unpower()
	{
		isEnpowered = false;
		ChangeDurability(0);
		SetConeierScreenShow(false);
		//Debug.Log($"Empowered roon {gameObject.name}");
	}

	private void Awake()
	{
		roomStatsController = GetComponentInChildren<RoomStatsController>(true);
		roomStatsController.SetRoomScript(this);
	}

	protected void MakeUICanvas()
	{
		try
		{
			GameObject.FindGameObjectsWithTag("UI_canvas").ToList().ForEach(x => x.GetComponent<Canvas>().worldCamera = Camera.main.GetComponentsInChildren<Camera>()[1]);
		}
		catch
		{
			GameObject.FindGameObjectsWithTag("UI_canvas").ToList().ForEach(x => x.GetComponent<Canvas>().worldCamera = Camera.main);
		}
	}

	protected virtual void Start()
	{
		MakeUICanvas();
		videoPlayers = GetComponentsInChildren<VideoPlayer>(true).ToList();
		TrySetVideoPlayers(false);
		if (progressbar) progressbar.gameObject.SetActive(false);
		workUI = GetComponentInChildren<RoomWorkUI>(true);
		audioSource = GetComponent<AudioSource>();
		statusPanel = GameManager.Instance.roomStatusListController.CreateRoomStatus(this);
		//animator = GetComponentInChildren<Animator>(); // we have a lot Animators, better to choose by hands;
		walkPoints = rawWalkPoints.ConvertAll(n => n.transform.position);
		roomStatsScreen = transform.Find("RoomInfo").gameObject;
		roomStatsScreen.SetActive(false);
		//roomBuildScreen = transform.Find("RoomBuildMode").gameObject;
		//roomBuildScreen.SetActive(false);
		hullPercentage = roomStatsScreen.transform.Find("hull%").GetComponent<TextMeshProUGUI>();
		hullBar = roomStatsScreen.transform.Find("Hull").transform;
		lamps = GameObject.FindGameObjectsWithTag("room_lamp").ToList().FindAll(g => g.transform.parent.IsChildOf(transform));
		lamps.ForEach(x => x.GetComponent<Renderer>().material.EnableKeyword("_EMISSION"));
		sparks = transform.GetComponentsInChildren<ParticleSystem>().Where(x => !x.CompareTag("permanentParticle")).ToList();
		defaultLampColor = lamps[0].GetComponent<Renderer>().material.color;
		if (resource != Resources.Cosmodrome)
		{
			baseOfRoom = transform.Find("base").gameObject;
		}
		else
		{
			baseOfRoom = transform.Find("aedasd").Find("base").gameObject;
		}
		defaultBaseColor = baseOfRoom.GetComponent<Renderer>().material.color;
		foreach (var button in GetComponentsInChildren<Button>().Where(x => !x.CompareTag("dont_hide_button")))
		{
			button.gameObject.SetActive(GameManager.Instance.mode == GameManager.Mode.Build);
		}
		switch (resource)
		{
			case Resources.Bed:
				workStr = "Прокачиваем медведей";
				GameManager.Instance.AddWorkStations(workStationsToOutline);
				GameManager.Instance.ChangeMaxBearAmount(5);
				workSound = SoundManager.Instance.livingRoomWorkSound;
				break;
			case Resources.Asteriy:
				workStr = "Переработка астерия";
				workSound = SoundManager.Instance.asteriumWorkSound;
				break;
			case Resources.Cosmodrome:
				workStr = "Добыча астерия";
				workSound = SoundManager.Instance.cosmodromeWorkSound;
				if (efficiencyDownPanel != null) SetWorkEfficiency(1, true, true);
				//StartCoroutine(ConstantResistorSetCaller(10));
				break;
			case Resources.Supply:
				workStr = "Монтаж сетей";
				workSound = SoundManager.Instance.supplyRoomWorkSound;
				break;
			case Resources.Build:
				workStr = "Строит";
				workSound = SoundManager.Instance.builderRoomWorkSound;
				break;
			case Resources.Energohoney:
				workStr = "Cинтез энергомеда";
				workSound = SoundManager.Instance.energohoneyRoomWorkSound;
				break;
			case Resources.Research:
				workStr = "Исследуем технологии";
				workSound = SoundManager.Instance.energohoneyRoomWorkSound;
				break;
				// in case Research - workStr = "Исследуем технологии"
				//default:
				//	if (workStationsToOutline.Count > 0)
				//	{
				//		GameManager.Instance.AddWorkStations(workStationsToOutline);
				//	}
				//	break;
		}
		audioSource.clip = workSound;
		if (!isEnpowered)
		{
			lamps.ForEach(y => y.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black));
			lamps.ForEach(z => z.GetComponentInChildren<Light>().enabled = false);
			baseOfRoom.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
			UpdateRoomHullView();
		}
		SetConeierScreenShow(isEnpowered);
		sparks.ForEach(y => y.Stop());
	}

	public void AssignWorkForSelectedBear()
	{
		try
		{
			GameManager.Instance.selectedUnit.GetComponent<UnitMovement>().currentRoom.GetComponent<BuilderRoom>().SetWait(false);
		}
		catch { }
		GameManager.Instance.WalkAndWork(GameManager.Instance.selectedUnit, gameObject);
		GameManager.Instance.HideAllAssignButtons();
	}

	public virtual void ShowButton()
	{
		if (isEnpowered && status == Status.Free && durability > 0 && resource != Resources.Asteriy)
		{
			//Debug.Log("<color=\"green\">" + gameObject.name + "</color>");
			assignmentButton.SetActive(true);
		}
		else
		{
			//Debug.Log("<color=\"orange\">" + gameObject.name + "</color>" + "<color=\"red\">" + " isEnpowered=" + (isEnpowered) + " status=" + (status == Status.Free) + " durability=" + (durability > 0) + "</color>");
		}
	}

	public virtual void HideButton()
	{
		if (resource != Resources.Asteriy)
		{
			assignmentButton.SetActive(false);
		}
	}

	public async void UpgradeRoom(GameObject button)
	{
		GameObject fixedBuilderRoom = null;
		foreach (GameObject room in GameManager.Instance.builderRooms)
		{
			if (room.GetComponent<BuilderRoom>().GetWait() && room.GetComponent<RoomScript>().fixedBear)
			{
				fixedBuilderRoom = room;
				break;
			}
		}
		if (fixedBuilderRoom == null)
		{
			Debug.Log("Нет свободных строительных комплексов!");
			EventManager.callWarning.Invoke($"Нет свободного <color=yellow>конструктора</color> в комплексе строительства!");
			return;
		}
		else if (!fixedBuilderRoom.GetComponent<BuilderRoom>().GetWait())
		{
			Debug.Log("Нет свободных строительных комплексов!");
			EventManager.callWarning.Invoke($"Нет свободного <color=yellow>конструктора</color> в комплексе строительства!");
			return;
		}

		int requireAsterium = ValuesHolder.RoomsBuildPrice[ConvertResourcesToRoomType(resource)][ResourceType.RepairAsteriumCost];
		int requireAstroluminite = ValuesHolder.RoomsBuildPrice[ConvertResourcesToRoomType(resource)][ResourceType.RepairAstroluminiteCost];

		if (await GameManager.Instance.GetAsteriy() >= requireAsterium &&
			await GameManager.Instance.GetAstroluminite() >= requireAstroluminite) //await GameManager.Instance.GetHoney() >= (30 + 10 * (level - 1))
		{
			fixedBuilderRoom.GetComponent<RoomScript>().SetStatus(Status.Busy);
			fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CannotBeSelected();
			fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().StopAllCoroutines();
			fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().MoveToRoom(this, true);
			//await GameManager.Instance.ChangeHoney(-(30 + 10 * (level - 1)), new Log
			//{
			//	comment = $"Consumed {(30 + 10 * (level - 1))} honey for upgrading {this.name} room",
			//
			//	player_name = GameManager.Instance.playerName,
			//	resources_changed = new Dictionary<string, float> { { "honey", -(30 + 10 * (level - 1)) } }
			//});

			await GameManager.Instance.ChangeAsteriy(-requireAsterium, new Log
			{
				comment = $"Consumed {requireAsterium} asterium for upgrading {this.name} room",

				player_name = GameManager.Instance.playerName,
				resources_changed = new Dictionary<string, float> { { "asterium", -requireAsterium } }
			});

			await GameManager.Instance.ChangeAstroluminite(-requireAstroluminite, new Log
			{
				comment = $"Consumed {requireAstroluminite} astroluminite for upgrading {this.name} room",

				player_name = GameManager.Instance.playerName,
				resources_changed = new Dictionary<string, float> { { "astroluminite", -requireAstroluminite } }
			});

			GameManager.Instance.uiResourceShower.UpdateIndicators();
			fixedBuilderRoom.GetComponent<BuilderRoom>().SetWait(false);
			StartCoroutine(Upgrade(button, fixedBuilderRoom));
		}
		else
		{
			Debug.Log("Не хватает ресов для починки!");
			//EventManager.callWarning.Invoke($"Не хватает <color=yellow>{Mathf.CeilToInt((30 + 10 * (level - 1)) - await GameManager.Instance.GetHoney())}</color> энергомеда для починки!");
			EventManager.callWarning.Invoke(NotEnoughResources(requireAsterium - await GameManager.Instance.GetAsteriy(), requireAstroluminite - await GameManager.Instance.GetAstroluminite()) + " для улучшения!");
			return;
		}
	}

	private string NotEnoughResources(float differenceAsterium, float differenceAstroluminite)
	{
		string toReturn = "Не хватает ";

		if (differenceAsterium > 0) toReturn += $"<color=yellow>{Mathf.CeilToInt(differenceAsterium)}</color> астерия, ";
		if (differenceAstroluminite > 0) toReturn += $"<color=yellow>{Mathf.CeilToInt(differenceAstroluminite)}</color> астролюминита, ";

		toReturn = toReturn[..^2];

		return toReturn;
	}

	private IEnumerator Upgrade(GameObject button, GameObject room)
	{
		upgradeBar.transform.parent.gameObject.SetActive(true);
		roomStatsController.SetStatsScreenShow(false);
		while (room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoom != this)
		{
			yield return null;
		}
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", true);
		float time = 5f;
		var newTimer = (float)time;
		while (newTimer > 0)
		{
			newTimer -= Time.deltaTime;
			upgradeBar.fillAmount = 1 - (newTimer / (float)time);
			yield return null;
		}
		upgradeBar.transform.parent.gameObject.SetActive(false);
		upgradeBar.fillAmount = 0;
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", false);
		room.GetComponent<RoomScript>().SetStatus(Status.Free);
		room.GetComponent<BuilderRoom>().SetWait(true, true);
		//durability = 1f;
		ChangeDurability(0);
		GameManager.Instance.WalkAndWork(room.GetComponent<BuilderRoom>().fixedBear, room);
		room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CanBeSelected();
		//status = Status.Free; // 😭;
		level += 1;
		if (level == 3)
		{
			button.GetComponent<Button>().enabled = false;
			button.GetComponentInChildren<TextMeshProUGUI>().text = "Максимальный уровень!";
		}
		UpdateRoomHullView();
		EventManager.onRoomUpgraded.Invoke();
		yield return null;
	}

	public void ToggleRoomStats(bool toggle)
	{
		roomStatsScreen.SetActive(toggle);
		UpdateRoomHullView();
		UpdateUpgradeView();
	}

	public async void UpdateUpgradeView()
	{
		if (level < 3)
		{
			int requireAsterium = ValuesHolder.RoomsBuildPrice[ConvertResourcesToRoomType(resource)][ResourceType.RepairAsteriumCost];
			int requireAstroluminite = ValuesHolder.RoomsBuildPrice[ConvertResourcesToRoomType(resource)][ResourceType.RepairAstroluminiteCost];

			var desiredButton = roomStatsScreen.GetComponentsInChildren<TextMeshProUGUI>(true).First(x => x.transform.parent.name.Contains("Improve"));
			if (await GameManager.Instance.GetAsteriy() < requireAsterium ||
				await GameManager.Instance.GetAstroluminite() < requireAstroluminite)//(currentHoney < (30 + 10 * (level - 1)));
			{
				//desiredButton.GetComponent<Button>() = false;
				desiredButton.text = NotEnoughResources(requireAsterium - await GameManager.Instance.GetAsteriy(), requireAstroluminite - await GameManager.Instance.GetAstroluminite()) + " для улучшения!";
			}
			else
			{
				desiredButton.text = $"Улучшить";
			}
		}
	}

	public void ToggleBuildStats(bool toggle)
	{
		if (level < 3)
		{
			UpdateUpgradeView();
		}
		if (!progressBar.transform.parent.gameObject.activeSelf && !upgradeBar.transform.parent.gameObject.activeSelf)
		{
			//roomBuildScreen.SetActive(toggle);
		}
	}

	public void UpdateRoomHullView()
	{
		roomStatsController.GetLevelText().text = $"Уровень: <color=yellow>{level}</color>";
		try
		{
			roomStatsScreen.transform.Find("hull%").GetComponent<TextMeshProUGUI>().text = $"{Mathf.RoundToInt((durability / 1f) * 100f)}%";
			roomStatsScreen.transform.Find("Hull").localScale = new Vector3(durability / 1f, 1, 1);
		}
		catch (Exception e)
		{
			Debug.Log($"An error occured during hull update! Error details: {e.Message}");
		}
	}

	public virtual void BuildRoom(GameObject button)
	{
		GameObject fixedBuilderRoom = null;
		foreach (GameObject room in GameManager.Instance.builderRooms)
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
			return;
		}
		GameManager.Instance.QueueBuildPos(button);
		GameManager.Instance.buildingScreen.SetActive(true);
		GameManager.Instance.buildingScreen.GetComponent<Animator>().SetTrigger("Show");
	}

	/// <summary>
	/// Start work at station by bear ( calls coroutine, can be interrupted by InterruptWork() )
	/// </summary>
	/// <param name="bear"></param>
	public virtual void StartWork(GameObject bear)
	{
		if (status != Status.Destroyed && isEnpowered)
		{
			switch (resource)
			{
				case Resources.Energohoney:
					break;
				case Resources.Asteriy:
					work = StartCoroutine(WorkStatus());
					roomStatsController.RefreshDescription();
					audioSource.Play();
					return;
				case Resources.Bed:
					break;
				case Resources.Build:
					break;
			}
			fixedBear = bear;
			fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
			fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(false);
			if (resource == Resources.Cosmodrome)
			{
				status = Status.Busy;
				statusPanel.UpdateStatus(status);
				fixedBear.GetComponent<UnitScript>().CannotBeSelected();
				GameManager.Instance.CheckEnpoweredAstriumRooms();
				cosmodromeSelectScreen.SetActive(true);
				return;
			}
			if (status == Status.Free)// && resource != Resources.Build)
			{
				work = StartCoroutine(WorkStatus());
				audioSource.Play();
				roomStatsController.RefreshDescription();
			}
		}
	}

	public void StartCosmodromeWork(int type)
	{
		switch (type)
		{
			case 0: // Asteriy
				if (GameManager.Instance.FlyForRawAsterium() && GameManager.Instance.season != GameManager.Season.Tide)
				{
					//timeShow.gameObject.SetActive(true);
					workUI.SetResultImage(GameManager.Instance.uiResourceShower.asteriyAmountText.transform.parent.parent.GetComponentsInChildren<Image>()[1].sprite);
					flyForType = FlyForType.Asterium;
					StartCoroutine(WorkStatus());
					roomStatsController.RefreshDescription();
				}
				else
				{
					if (GameManager.Instance.season == GameManager.Season.Tide)
					{
						EventManager.callWarning.Invoke("Невозможно отправить! Сейчас фаза <color=yellow>прилива</color>.");
					}
					if (!GameManager.Instance.FlyForRawAsterium())
					{
						EventManager.callWarning.Invoke("Невозможно отправить! Все комплексы переработки астерия <color=yellow>заняты</color>.");
					}
				}
				break;
			case 1: // Astroluminite
				if (GameManager.Instance.season != GameManager.Season.Tide)
				{
					//timeShow.gameObject.SetActive(true);
					workUI.SetResultImage(GameManager.Instance.uiResourceShower.astroluminiteAmountText.transform.parent.parent.GetComponentsInChildren<Image>()[1].sprite); //im sorry 🤡
					flyForType = FlyForType.Astroluminite;
					StartCoroutine(WorkStatus());
					roomStatsController.RefreshDescription();
				}
				else
				{
					if (GameManager.Instance.season == GameManager.Season.Tide)
					{
						EventManager.callWarning.Invoke("Невозможно отправить! Сейчас фаза <color=yellow>прилива</color>.");
					}
				}
				break;
		}
	}

	public enum FlyForType
	{
		None,
		Asterium,
		Astroluminite
	}

	public async Task StartShopWork()
	{
		if (GameManager.Instance.FlyForRawAsterium() && GameManager.Instance.season != GameManager.Season.Tide)
		{
			//timeShow.gameObject.SetActive(true);
			StartCoroutine(WorkStatus());
			roomStatsController.RefreshDescription();
		}
	}

	/// <summary>
	/// Stops work
	/// </summary>
	public virtual void InterruptWork()
	{
		if (work != null)
		{
			StopCoroutine(work);
		}
		work = null;

		fixedBear.GetComponent<UnitScript>().CanBeSelected();
		if (resource == Resources.Cosmodrome)
		{
			//timeShow.gameObject.SetActive(false);
			cosmodromeSelectScreen.SetActive(false);
			status = Status.Free;
			statusPanel.UpdateStatus(status);
		}
		fixedBear.GetComponent<UnitScript>().SetWorkStr("Не занят");
		fixedBear = null;
		roomStatsController.RefreshDescription();
	}

	protected virtual IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		statusPanel.UpdateStatus(status);
		animator.SetTrigger("StartWork");
		if (resource != Resources.Asteriy)
		{
			if (resource != Resources.Cosmodrome) fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
			fixedBear.GetComponent<UnitScript>().SetBusy(true);
			fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		}
		switch (resource)
		{
			case Resources.Asteriy:
				TrySetVideoPlayers(true);
				timer = ValuesHolder.StandartInteractionTimeAsteriumComplex;
				workUI.StartWork(timer, ValuesHolder.AsteriumAmountByOneInteraction, GameManager.Instance.uiResourceShower.asteriyAmountText.transform);
				while (timer > 0)
				{
					//timeShow.text = SecondsToTimeToShow(timer);
					timer -= Time.deltaTime;
					yield return null;
				}
				//timeShow.text = "";
				GameManager.Instance.WithdrawRawAsterium();
				GameManager.Instance.ChangeAsteriy(ValuesHolder.AsteriumAmountByOneInteraction, new Log
				{
					comment = $"Added {ValuesHolder.AsteriumAmountByOneInteraction} asterium to player {GameManager.Instance.playerName} for processing raw asterium from spaceship",
					player_name = GameManager.Instance.playerName,

					resources_changed = new Dictionary<string, float> { { "asterium", ValuesHolder.AsteriumAmountByOneInteraction } }
				});
				TrySetVideoPlayers(false);
				isReadyForWork = false;
				GameManager.Instance.uiResourceShower.UpdateIndicators();
				break;
			case Resources.Cosmodrome:
				cosmodromeSelectScreen.SetActive(false);
				fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
				//timer = 45f;
				//(1 - 0.05f * fixedBear.GetComponent<UnitScript>().level)
				if ((fixedBear.GetComponent<UnitScript>().job == Qualification.researcher) && (level != 1 && fixedBear.GetComponent<UnitScript>().level != 1))
				{
					//timer = 45f * (1 - 0.25f * (level - 1)) * (1 - 0.05f * fixedBear.GetComponent<UnitScript>().level);
					//timer = StandartInteractionTime * (1 - (SpeedByBearLevelCoef - 1) * fixedBear.GetComponent<UnitScript>().level) * SpeedByUsingSuitableBearCoef * (level > 1 ? (1 - (1 - SpeedByRoomLevelCoef) * level) : 1);
					timer = GetModifiedInteractionTime(fixedBear.GetComponent<UnitScript>().level);
					fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(true);
				}
				else
				{
					//timer = 45f * 1.25f * (1 - 0.25f * (level - 1));
					//timer = StandartInteractionTime * (level > 1 ? (1 - (1 - SpeedByRoomLevelCoef) * level) : 1);
					timer = ValuesHolder.StandartInteractionTime;
				}
				if (fixedBear.GetComponent<UnitScript>().isBoosted)
				{
					timer *= 0.9f;
				}
				timer *= 1 + (1 - efficientyCoeficent); // for RESISTORS exercise;
				(workUI as FluidWorkUI).StartWork(timer, 20, GameManager.Instance.uiResourceShower.astroluminiteAmountText.transform); // I wanted to placer ValuesHolder var where "20", but there is 20 for astroluminite??? idk, better to leave unchanged
				fixedBear.SetActive(false);
				while (timer > 0)
				{
					//timeShow.text = SecondsToTimeToShow(timer);
					timer -= Time.deltaTime;
					if (timer <= 10f)
					{
						animator.SetTrigger("EndWork");
						break;
					}
					yield return null;
				}
				while (timer > 0)
				{
					//timeShow.text = SecondsToTimeToShow(timer);
					timer -= Time.deltaTime;
					yield return null;
				}
				fixedBear.SetActive(true);
				switch (flyForType)
				{
					case FlyForType.Asterium:
						GameManager.Instance.DeliverRawAsterium();
						break;
					case FlyForType.Astroluminite:
						GameManager.Instance.ChangeAstroluminite(ValuesHolder.AstroluminiteAmountByOneInteraction, new Log
						{
							comment = $"Added {ValuesHolder.AstroluminiteAmountByOneInteraction} astroluminite to player {GameManager.Instance.playerName} from spaceship",
							player_name = GameManager.Instance.playerName,

							resources_changed = new Dictionary<string, float> { { "astroluminite", ValuesHolder.AstroluminiteAmountByOneInteraction } }
						});
						break;
				}
				GameManager.Instance.uiResourceShower.UpdateIndicators();

				if (fixedBear.GetComponent<UnitScript>().job == Qualification.researcher)
				{
					fixedBear.GetComponent<UnitScript>().LevelUpBear();
				}
				//timeShow.gameObject.SetActive(false);
				cosmodromeSelectScreen.SetActive(false);
				fixedBear.GetComponent<UnitScript>().CanBeSelected();
				break;
		}
		if (resource != Resources.Asteriy)
		{
			fixedBear.GetComponent<UnitScript>().SetBusy(false);
		}
		if (fixedBear != null)
		{
			fixedBear.GetComponent<UnitScript>().SetWorkStr("Не занят");
			fixedBear.GetComponent<UnitScript>().CanBeSelected();
			fixedBear = null;
		}
		status = Status.Free;
		statusPanel.UpdateStatus(status);
		if (resource != Resources.Cosmodrome)
		{
			animator.SetTrigger("EndWork");
		}
		audioSource.Stop();
		roomStatsController.RefreshDescription();
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("unit"))
		{
			other.GetComponent<UnitMovement>().currentRoom = this;
			if (other.GetComponent<UnitMovement>().currentElevator == null && connectedElevators != null)
			{
				other.GetComponent<UnitMovement>().currentElevator = connectedElevators[0];
			}
		}
	}

	protected string SecondsToTimeToShow(float seconds) // left - minutes, right - seconds. no hours.
	{
		return (int)seconds / 60 + ":" + (((int)seconds % 60 < 10) ? "0" + (int)seconds % 60 : (int)seconds % 60);
	}

	public List<Vector3> GetWalkPoints()
	{
		return walkPoints;
	}

	public virtual void SetDurability(float hp) => ChangeDurability(hp - durability);

	/// <summary>
	/// Changes durability (wow) "-" to damage, "+" to heal
	/// </summary>
	/// <param name="hp"></param>
	public virtual void ChangeDurability(float hp)
	{
		if (resource != Resources.Build) durability += hp;
		if (durability <= 0)
		{
			status = Status.Destroyed;
			try
			{
				statusPanel.UpdateStatus(status);
			}
			catch
			{
				//Debug.Log("No status panel present at this moment!");
			}
			SetLampsOn(false);
			if (blinks != null)
			{
				StopCoroutine(blinks);
				blinks = null;
			}
			durability = Mathf.Clamp(durability, 0f, 1f);
			UpdateRoomHullView();
			return;
		}
		durability = Mathf.Clamp(durability, 0f, 1f);
		try
		{
			statusPanel.UpdateDurability(durability);
		}
		catch (Exception e)
		{
			//Debug.Log("No statusPanel present at this moment! " + name);
		}
		if (!isEnpowered)
		{
			SetLampsOn(false);
			UpdateRoomHullView();
			return;
		}
		try
		{
			if (durability > 0.5f)
			{
				baseOfRoom.GetComponent<Renderer>().material.SetColor("_EmissionColor", defaultBaseColor);
				sparks.ForEach(x => x.Stop());
				SetLampsColor(defaultLampColor);
				SetLampsOn(true);
				if (blinks != null)
				{
					StopCoroutine(blinks);
					blinks = null;
				}
			}
			else if (durability > 0.3f)
			{
				SetLampsColor(defaultLampColor);
				SetLampsOn(true);
				baseOfRoom.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow);
				sparks.ForEach(x => x.Stop());
				blinks ??= StartCoroutine(LampsBlinking());
			}
			else if (durability > 0.15f)
			{
				SetLampsColor(defaultLampColor);
				SetLampsOn(true);
				baseOfRoom.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
				sparks.ForEach(x => x.Play());
				blinks ??= StartCoroutine(LampsBlinking());
			}
			else if (durability > 0f)
			{
				baseOfRoom.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
				sparks.ForEach(x => x.Play());
				SetLampsColor(Color.red);
				SetLampsOn(true);
				if (blinks != null)
				{
					StopCoroutine(blinks);
					blinks = null;
				}
			}
			else
			{
				SetLampsOn(false);
				if (blinks != null)
				{
					StopCoroutine(blinks);
					blinks = null;
				}
				InterruptWork();
				animator.SetTrigger("EndWork");
				statusPanel.UpdateDurability(durability);
			}
		}
		catch (Exception e)
		{
			Debug.Log($"An error occured during durability change! Error details: {e.Message}");
		}
		UpdateRoomHullView();
		try { statusPanel.UpdateDurability(durability); } catch { }
	}

	private IEnumerator LampsBlinking()
	{
		while (true)
		{
			for (int i = 0; i < Random.Range(0, 3); i++)
			{
				SetLampsOn(false);
				yield return new WaitForSeconds(Random.value / 4);
				SetLampsOn(true);
			}
			yield return new WaitForSeconds(5 + Random.value * 5);
		}
	}

	private void SetLampsOn(bool set)
	{
		if (set)
		{
			lamps.ForEach(x => x.GetComponent<Renderer>().material.EnableKeyword("_EMISSION"));
			lamps.ForEach(y => y.GetComponentInChildren<Light>().enabled = true);
		}
		else
		{
			lamps.ForEach(x => x.GetComponent<Renderer>().material.DisableKeyword("_EMISSION"));
			lamps.ForEach(y => y.GetComponentInChildren<Light>().enabled = false);
		}
	}

	private void SetLampsColor(Color color)
	{
		lamps.ForEach(y => y.GetComponent<Renderer>().material.SetColor("_EmissionColor", color));
		lamps.ForEach(x => x.GetComponentInChildren<Light>().color = color);
	}

	/// <summary>
	/// Repairs room to full
	/// </summary>
	public async void RepairRoom()
	{
		GameObject fixedBuilderRoom = null;
		foreach (GameObject room in GameManager.Instance.builderRooms)
		{
			if (room.GetComponent<BuilderRoom>().GetWait() && room.GetComponent<RoomScript>().fixedBear)
			{
				fixedBuilderRoom = room;
				break;
			}
		}
		if (fixedBuilderRoom == null)
		{
			Debug.Log("Медведь занят!");
			EventManager.callWarning.Invoke($"Нет свободного <color=yellow>конструктора</color> в комплексе строительства!");
			return;
		}
		else if (!fixedBuilderRoom.GetComponent<BuilderRoom>().GetWait())
		{
			Debug.Log("Нет свободных строительных комплексов!");
			EventManager.callWarning.Invoke($"Нет свободного <color=yellow>конструктора</color> в комплексе строительства!");
			return;
		}
		if (await GameManager.Instance.GetAsteriy() >= ValuesHolder.RepairCost)
		{
			GameManager.Instance.ChangeAsteriy(-ValuesHolder.RepairCost, new Log
			{
				comment = $"Consumed {ValuesHolder.RepairCost} asterium from player {GameManager.Instance.playerName} to repair {gameObject.name} room",
				player_name = GameManager.Instance.playerName,

				resources_changed = new Dictionary<string, float> { { "asterium", -ValuesHolder.RepairCost } }
			});
			fixedBuilderRoom.GetComponent<RoomScript>().SetStatus(Status.Busy);
			GameManager.Instance.uiResourceShower.UpdateIndicators();
			int timeToRepair = (int)((1 - durability) * 100 / ValuesHolder.RepairSpeed); //((1 - durability) * 100 / 3);
			fixedBuilderRoom.GetComponent<BuilderRoom>().SetWait(false);
			fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CannotBeSelected();
			if (fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoutine != null)
			{
				fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().StopCoroutine(fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoutine);
			}
			fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoutine = null;
			fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().MoveToRoom(this, true);
			StartCoroutine(Repair(timeToRepair, fixedBuilderRoom));
		}
		else
		{
			Debug.Log("Не хватает ресов для починки!");
			EventManager.callWarning.Invoke($"Не хватает <color=yellow>{ValuesHolder.RepairCost - await GameManager.Instance.GetAsteriy()}</color> астерия для починки!");
			return;
		}
	}

	private IEnumerator Repair(int time, GameObject room)
	{
		roomStatsController.SetStatsScreenShow(false);
		progressBar.transform.parent.gameObject.SetActive(true);
		while (room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoom != this)
		{
			yield return null;
		}
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", true);
		var newTimer = (float)time;
		while (newTimer > 0)
		{
			newTimer -= Time.deltaTime;
			progressBar.fillAmount = 1 - (newTimer / (float)time);
			yield return null;
		}
		progressBar.transform.parent.gameObject.SetActive(false);
		progressBar.fillAmount = 0;
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", false);
		//room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().MoveToRoom(room.GetComponent<RoomScript>());
		room.GetComponent<BuilderRoom>().SetWait(true, true);
		room.GetComponent<RoomScript>().SetStatus(Status.Free);
		durability = 1f;
		ChangeDurability(0);
		GameManager.Instance.WalkAndWork(room.GetComponent<BuilderRoom>().fixedBear, room);
		room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CanBeSelected();
		//status = Status.Free; // ????? why it was here???
		statusPanel.UpdateStatus(status);
	}

	public void SetStatus(Status status)
	{
		this.status = status;
		statusPanel.UpdateStatus(status);
	}

	public virtual void ShowSetPipesButtonScreen()
	{
		SetConeierScreen(true);
	}

	public virtual bool CheckIfSolved()
	{
		if (isSolved) return true;
		if (coneierScreen != null)
		{
			try
			{
				return !coneierScreen.GetComponent<Button>().interactable;
			}
			catch (Exception ex)
			{
				Debug.Log("Error when check is solved! Returning false. Description: " + ex.Message);
				return false;
			}
		}
		else
		{
			return isSolved;
		}
	}

	public virtual void SetIsSolved(bool set) => isSolved = set;

	public virtual void HideSetPipesButtonScreen()
	{
		SetConeierScreen(false);
		StartCoroutine(DelayedSetConeierScreen(true, 0.5f));
	}

	private IEnumerator DelayedSetConeierScreen(bool set, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		SetConeierScreen(set);
	}

	public virtual void SetPipes()
	{
		Camera.main.GetComponent<CameraController>().GoToTaskPoint(cameraPoint.position, cameraAngle, true);
		if (resource == Resources.Asteriy) SetTakeALookButtonShow(true);
		MenuManager.Instance.CallProblemSolver(MenuManager.ProblemType.SetFurnaces, this);
		HideSetPipesButtonScreen();
		MakeUICanvas();
	}

	public void SetTakeALookButtonShow(bool set) => takeALookButton.SetActive(set);

	private IEnumerator ConstantResistorSetCaller(float cooldownTime) // No more constant. Just calls chain with infinity loop.
	{
		//while (true)
		//{
		//	yield return new WaitForSeconds(cooldownTime);
		//	SetWorkEfficiency(1 - 0.3f);
		//	//SetResistors();
		//	waitForPermissionToContinue = true;
		//	yield return PermissionToContinueWaiter();
		//}
		yield return null;
	}

	private IEnumerator PermissionToContinueWaiter()
	{
		while (waitForPermissionToContinue)
		{
			yield return null;
		}
	}

	public void GivePermissionToContinue()
	{
		waitForPermissionToContinue = false;
	}

	public void SetResistors() // God is displeased;
	{
		MenuManager.Instance.CallProblemSolver(MenuManager.ProblemType.SetCosmodrome, this);
	}

	public void SetConeierScreen(bool set)
	{
		if (coneierScreen != null)
		{
			coneierScreen.GetComponentInChildren<Button>().interactable = set;
		}
		else
		{
			Debug.Log("Не назначен coneierScreen в комплексе: <color=\"orange\">" + gameObject.name + "</color>");
		}
	}

	public void SetConeierScreenShow(bool set)
	{
		if (coneierScreen != null)
		{
			Animator efficiencyAnim = efficiencyDownPanel.GetComponent<Animator>();
			if (set)
			{
				if (efficiencyAnim.GetCurrentAnimatorStateInfo(0).IsName("HideCompletely") && !CheckIfSolved())
					efficiencyAnim.SetTrigger("UnHideCompletely");
			}
			else
			{
				if (!efficiencyAnim.GetCurrentAnimatorStateInfo(0).IsName("HideCompletely"))
					efficiencyAnim.SetTrigger("HideCompletely");
			}
		}
		else
		{
			Debug.Log("Не назначен coneierScreen в комплексе: <color=\"orange\">" + gameObject.name + "</color>");
		}
	}

	public RoomStatsController GetRoomStatsController()
	{
		return roomStatsController;
	}

	public GameObject GetRoomStatsScreen()
	{
		return roomStatsScreen;
	}

	public float GetInteractionTime()
	{
		return StandartInteractionTime;
	}

	public bool GetIsEfficBearWorking()
	{
		if (fixedBear == null) return false;
		Qualification job = fixedBear.GetComponent<UnitScript>().job;
		switch (resource)
		{
			case Resources.Energohoney:
				return job == Qualification.beekeeper;
			case Resources.Supply:
				return job == Qualification.coder;
			case Resources.Cosmodrome:
				return job == Qualification.researcher;
			case Resources.Research:
				return job == Qualification.bioengineer;
			case Resources.Bed:
				return job == Qualification.creator;
			case Resources.Asteriy:
				return false;
			case Resources.Build:
				return job == Qualification.builder;
			default:
				Debug.Log("<color=red>ЧТО-ТО НЕ ТАК</color>");
				return false;
		}
	}

	public enum Resources
	{
		Energohoney,
		Asteriy,
		Cosmodrome,
		Bed,
		Build,
		Supply,
		Research
	}

	public enum Status
	{
		Free,
		Busy,
		Destroyed
	}

	protected float GetModifiedInteractionTime(float bearLevel) => ValuesHolder.StandartInteractionTime * Mathf.Pow(ValuesHolder.InteractionSpeedMultiplyerByLevel, bearLevel) * Mathf.Pow(ValuesHolder.InteractionSpeedMultiplyerByGrade, level);

	public void GetPrices(out int asteriumCost, out int honeyCost, out int astroluminiteCost) => GetPrices(out asteriumCost, out honeyCost, out astroluminiteCost, resource);
	public void GetPrices(out int asteriumCost, out int honeyCost, out int astroluminiteCost, Resources resource)
	{
		switch (resource)
		{
			case Resources.Energohoney:
				asteriumCost = ValuesHolder.RoomsBuildPrice[RoomType.Energohoney][ResourceType.AsteriumPrice];
				honeyCost = ValuesHolder.RoomsBuildPrice[RoomType.Energohoney][ResourceType.EnergohoneyPrice];
				astroluminiteCost = ValuesHolder.RoomsBuildPrice[RoomType.Energohoney][ResourceType.AstroluminitePrice];
				break;

			case Resources.Asteriy:
				asteriumCost = ValuesHolder.RoomsBuildPrice[RoomType.Asterium][ResourceType.AsteriumPrice];
				honeyCost = ValuesHolder.RoomsBuildPrice[RoomType.Asterium][ResourceType.EnergohoneyPrice];
				astroluminiteCost = ValuesHolder.RoomsBuildPrice[RoomType.Asterium][ResourceType.AstroluminitePrice];
				break;

			case Resources.Cosmodrome:
				asteriumCost = ValuesHolder.RoomsBuildPrice[RoomType.Cosmodrome][ResourceType.AsteriumPrice];
				honeyCost = ValuesHolder.RoomsBuildPrice[RoomType.Cosmodrome][ResourceType.EnergohoneyPrice];
				astroluminiteCost = ValuesHolder.RoomsBuildPrice[RoomType.Cosmodrome][ResourceType.AstroluminitePrice];
				break;

			case Resources.Bed:
				asteriumCost = ValuesHolder.RoomsBuildPrice[RoomType.Bed][ResourceType.AsteriumPrice];
				honeyCost = ValuesHolder.RoomsBuildPrice[RoomType.Bed][ResourceType.EnergohoneyPrice];
				astroluminiteCost = ValuesHolder.RoomsBuildPrice[RoomType.Bed][ResourceType.AstroluminitePrice];
				break;

			case Resources.Build:
				asteriumCost = ValuesHolder.RoomsBuildPrice[RoomType.Build][ResourceType.AsteriumPrice];
				honeyCost = ValuesHolder.RoomsBuildPrice[RoomType.Build][ResourceType.EnergohoneyPrice];
				astroluminiteCost = ValuesHolder.RoomsBuildPrice[RoomType.Build][ResourceType.AstroluminitePrice];
				break;

			case Resources.Supply:
				asteriumCost = ValuesHolder.RoomsBuildPrice[RoomType.Supply][ResourceType.AsteriumPrice];
				honeyCost = ValuesHolder.RoomsBuildPrice[RoomType.Supply][ResourceType.EnergohoneyPrice];
				astroluminiteCost = ValuesHolder.RoomsBuildPrice[RoomType.Supply][ResourceType.AstroluminitePrice];
				break;

			case Resources.Research:
				asteriumCost = ValuesHolder.RoomsBuildPrice[RoomType.Research][ResourceType.AsteriumPrice];
				honeyCost = ValuesHolder.RoomsBuildPrice[RoomType.Research][ResourceType.EnergohoneyPrice];
				astroluminiteCost = ValuesHolder.RoomsBuildPrice[RoomType.Research][ResourceType.AstroluminitePrice];
				break;

			default:
				Debug.Log("<color=red>ЧТО-ТО НЕ ТАК</color>");
				asteriumCost = 0;
				honeyCost = 0;
				astroluminiteCost = 0;
				break;
		}
	}

	private RoomType ConvertResourcesToRoomType(Resources resource)
	{
		return resource switch
		{
			Resources.Energohoney => RoomType.Energohoney,
			Resources.Asteriy => RoomType.Asterium,
			Resources.Cosmodrome => RoomType.Cosmodrome,
			Resources.Bed => RoomType.Bed,
			Resources.Build => RoomType.Build,
			Resources.Supply => RoomType.Supply,
			Resources.Research => RoomType.Research,
			_ => throw new ArgumentException(nameof(ConvertResourcesToRoomType), "Unknown room or it's elevator"),
		};
	}

	public void TrySetVideoPlayers(bool set)
	{
		if (videoPlayers.Count > 0)
		{
			videoPlayers.ForEach(vp => vp.enabled = set);
		}
		else
		{
			Debug.Log("Нет видео для: " + name);
		}
	}

	public Vector3 GetCameraPosition() => cameraPoint.position;
	public Vector3 GetCameraAngle() => cameraAngle;
}