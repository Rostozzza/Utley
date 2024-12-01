using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;
using UnityEngine.AI;
using System.Linq;

public class RoomScript : MonoBehaviour
{
	[SerializeField] public Status status;
	[SerializeField] public Resources resource;
	[SerializeField] public GameObject leftDoor;
	[SerializeField] public GameObject rightDoor;
	[SerializeField] public bool hasLeftDoor;
	[SerializeField] public bool hasRightDoor;
	[SerializeField] private GameObject roomStatsScreen;
	[SerializeField] private GameObject roomBuildScreen;
	public Room roomModel;

	public List<Elevator> connectedElevators;
	public List<RoomScript> connectedRooms;
	private Coroutine work;
	//[SerializeField] private Transform[] rawWalkPoints; // for energohoney 1 - 3 is paseka, 4 is generator
	[SerializeField] private List<Transform> rawWalkPoints;
	//private Vector3[] walkPoints;
	private List<Vector3> walkPoints;
	[SerializeField] protected TextMeshProUGUI timeShow;
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
	protected Animator animator;

	[Header("Asterium settings")]
	public bool isReadyForWork = false;
	
	protected virtual void Start()
	{
		animator = GetComponentInChildren<Animator>();
		walkPoints = rawWalkPoints.ConvertAll(n => n.transform.position);
		roomStatsScreen = transform.Find("RoomInfo").gameObject;
		roomStatsScreen.SetActive(false);
		roomBuildScreen = transform.Find("RoomBuildMode").gameObject;
		roomBuildScreen.SetActive(false);
		hullPercentage = roomStatsScreen.transform.Find("hull%").GetComponent<TextMeshProUGUI>();
		levelText = roomStatsScreen.transform.Find("Level (1)").GetComponent<TextMeshProUGUI>();
		hullBar = roomStatsScreen.transform.Find("Hull").transform;
		lamps = GameObject.FindGameObjectsWithTag("room_lamp").ToList().FindAll(g => g.transform.parent.IsChildOf(transform));
		lamps.ForEach(x => x.GetComponent<Renderer>().material.EnableKeyword("_EMISSION"));
		sparks = transform.GetComponentsInChildren<ParticleSystem>().ToList();
		defaultLampColor = lamps[0].GetComponent<Renderer>().material.color;
		baseOfRoom = transform.Find("base").gameObject;
		defaultBaseColor = baseOfRoom.GetComponent<Renderer>().material.color;
		foreach (var button in GetComponentsInChildren<Button>())
		{
			button.gameObject.SetActive(GameManager.Instance.mode == GameManager.Mode.Build);
		}
		switch (resource)
		{
			case Resources.Bed:
				GameManager.Instance.AddWorkStations(workStationsToOutline);
				GameManager.Instance.ChangeMaxBearAmount(6);
				break;
			default:
				if (workStationsToOutline.Count > 0)
				{
					GameManager.Instance.AddWorkStations(workStationsToOutline);
				}
				break;
		}
		sparks.ForEach(y => y.Stop());
	}

	public async void UpgradeRoom(GameObject button)
	{
		GameObject fixedBuilderRoom = null;
		foreach (GameObject room in GameManager.Instance.builderRooms)
		{
			if ((room.GetComponent<RoomScript>().status == Status.Free) && room.GetComponent<RoomScript>().fixedBear)
			{
				room.GetComponent<RoomScript>().SetStatus(Status.Busy);
				fixedBuilderRoom = room;
				break;
			}
		}
		if (fixedBuilderRoom == null)
		{
			Debug.Log("Нет свободных строительных комплексов!");
			return;
		}
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().StopAllCoroutines();
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().MoveToRoom(this);

		if (await GameManager.Instance.GetHoney() >= (30 + 10 * (level - 1)))
		{
			GameManager.Instance.ChangeHoney(-(30 + 10 * (level - 1)));
			GameManager.Instance.uiResourceShower.UpdateIndicators();
			StartCoroutine(Upgrade(button, fixedBuilderRoom));
		}
	}

	private IEnumerator Upgrade(GameObject button, GameObject room)
	{
		while (room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoutine != null)
		{
			yield return null;
		}
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", true);
		yield return new WaitForSeconds(5);
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", false);
		level += 1;
		if (level == 3)
		{
			button.GetComponent<Button>().enabled = false;
			button.GetComponentInChildren<TextMeshProUGUI>().text = "Максимальный уровень!";
		}
		UpdateRoomHullView();
		yield return null;
	}

	public void ToggleRoomStats(bool toggle)
	{
		roomStatsScreen.SetActive(toggle);
		UpdateRoomHullView();
	}

	public void ToggleBuildStats(bool toggle)
	{
		roomBuildScreen.SetActive(toggle);
	}


	public void UpdateRoomHullView()
	{
		levelText.text = "";
		for (int i = 0; i < level; i++)
		{
			levelText.text += "I";
		}
		roomStatsScreen.transform.Find("hull%").GetComponent<TextMeshProUGUI>().text = $"{Mathf.RoundToInt((durability / 1f) * 100f)}%";
		roomStatsScreen.transform.Find("Hull").localScale = new Vector3(durability/1f,1,1);
	}

	public virtual void BuildRoom(GameObject button)
	{
		GameManager.Instance.QueueBuildPos(button);
		GameManager.Instance.buildingScreen.SetActive(true);
	}

	/// <summary>
	/// Start work at station by bear ( calls coroutine, can be interrupted by InterruptWork() )
	/// </summary>
	/// <param name="bear"></param>
	public virtual void StartWork(GameObject bear)
	{
		if (status != Status.Destroyed)
		{
			switch (resource)
			{
				case Resources.Energohoney:
					break;
				case Resources.Asteriy:
					work = StartCoroutine(WorkStatus());
					return;
				case Resources.Bed:
					break;
				case Resources.Build:
					break;
			}
			fixedBear = bear;
			if (resource == Resources.Cosmodrome)
			{
				status = Status.Busy;
				fixedBear.GetComponent<UnitScript>().CannotBeSelected();
				timeShow.transform.parent.gameObject.SetActive(true);
				return;
			}
			if (status == Status.Free && resource != Resources.Build)
			{
				work = StartCoroutine(WorkStatus());
			}
		}
	}

	public void StartCosmodromeWork()
	{
		if (GameManager.Instance.FlyForRawAsterium() && GameManager.Instance.season != GameManager.Season.Tide)
		{
			timeShow.gameObject.SetActive(true);
			StartCoroutine(WorkStatus());
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
			timeShow.gameObject.SetActive(false);
			timeShow.transform.parent.gameObject.SetActive(false);
			status = Status.Free;
		}
		fixedBear = null;
	}

	protected virtual IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		animator.SetTrigger("StartWork");
		if (resource != Resources.Asteriy)
		{
			fixedBear.GetComponent<UnitScript>().SetBusy(true);
			fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		}
		switch (resource)
		{
			case Resources.Asteriy:
				timer = 45f;
				while (timer > 0)
				{
					timeShow.text = SecondsToTimeToShow(timer);
					timer -= Time.deltaTime;
					yield return null;
				}
				timeShow.text = "";
				GameManager.Instance.WithdrawRawAsterium();
				GameManager.Instance.ChangeAsteriy(20);
				isReadyForWork = false;
				GameManager.Instance.uiResourceShower.UpdateIndicators();
				break;
			case Resources.Cosmodrome:
				timer = 3f;
				if (fixedBear.GetComponent<UnitScript>().isBoosted)
				{
					timer *= 0.9f;
				}
				while (timer > 0)
				{
					timeShow.text = SecondsToTimeToShow(timer);
					timer -= Time.deltaTime;
					yield return null;
				}
				GameManager.Instance.DeliverRawAsterium();
				timeShow.gameObject.SetActive(false);
				timeShow.transform.parent.gameObject.SetActive(false);
				fixedBear.GetComponent<UnitScript>().CanBeSelected();
				break;
		}
		fixedBear.GetComponent<UnitScript>().SetBusy(false);
		if (fixedBear != null)
		{
			fixedBear.GetComponent<UnitScript>().CanBeSelected();
			fixedBear = null;
		}
		status = Status.Free;
		animator.SetTrigger("EndWork");
	}

	protected string SecondsToTimeToShow(float seconds) // left - minutes, right - seconds. no hours.
	{
		return (int)seconds / 60 + ":" + (((int)seconds % 60 < 10) ? "0" + (int)seconds % 60 : (int)seconds % 60);
	}

	public List<Vector3> GetWalkPoints()
	{
		return walkPoints;
	}

	/// <summary>
	/// Changes durability (wow) "-" to damage, "+" to heal
	/// </summary>
	/// <param name="hp"></param>
	public void ChangeDurability(float hp)
	{
		Coroutine blinks = null;
		durability += hp;
		if (durability <= 0)
		{
			status = Status.Destroyed;
		}
		durability = Mathf.Clamp(durability, 0f, 1f);
		if (durability > 0.5f)
		{
			baseOfRoom.GetComponent<Renderer>().material.SetColor("_EmissionColor", defaultBaseColor);
			sparks.ForEach(x => x.Stop());
			lamps.ForEach(y => y.GetComponent<Renderer>().material.SetColor("_EmissionColor", defaultLampColor));
			if (blinks != null)
			{
				StopCoroutine(blinks);
				blinks = null;
				lamps.ForEach(x => x.GetComponent<Renderer>().material.EnableKeyword("_EMISSION"));
			}
		}
		else if (durability > 0.3f)
		{
			baseOfRoom.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow);
			if (blinks == null)
			{
				blinks = StartCoroutine(LampsBlinking());
			}
		}
		else if (durability > 0.15f)
		{
			baseOfRoom.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
			if (blinks == null)
			{
				sparks.ForEach(z => z.Play());
				blinks = StartCoroutine(LampsBlinking());
			}
		}
		else if (durability > 0)
		{
			baseOfRoom.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
			sparks.ForEach(x => x.Stop());
			lamps.ForEach(y => y.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red));
			if (blinks != null)
			{
				StopCoroutine(blinks);
				blinks = null;
				lamps.ForEach(x => x.GetComponent<Renderer>().material.EnableKeyword("_EMISSION"));
			}
		}
		else 
		{
			lamps.ForEach(y => y.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black));
		}
		UpdateRoomHullView();
	}

	private IEnumerator LampsBlinking()
	{
		while (true)
		{
			for (int i = 0; i < Random.Range(0, 3); i++)
			{
				lamps.ForEach(x => x.GetComponent<Renderer>().material.DisableKeyword("_EMISSION"));
				yield return new WaitForSeconds(Random.value / 4);
				lamps.ForEach(x => x.GetComponent<Renderer>().material.EnableKeyword("_EMISSION"));
			}
			yield return new WaitForSeconds(5 + Random.value * 5);
		}
	}

    /// <summary>
	/// Repairs room to full for 10 asterium
	/// </summary>
	public async void RepairRoom()
	{
		GameObject fixedBuilderRoom = null;
		foreach (GameObject room in GameManager.Instance.builderRooms)
		{
			if ((room.GetComponent<RoomScript>().status == Status.Free) && room.GetComponent<RoomScript>().fixedBear)
			{
				room.GetComponent<RoomScript>().SetStatus(Status.Busy);
				fixedBuilderRoom = room;
				break;
			}
		}
		if (fixedBuilderRoom == null)
		{
			Debug.Log("Нет свободных строительных комплексов!");
			return;
		}
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().StopAllCoroutines();
		fixedBuilderRoom.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().MoveToRoom(this);

        
		if (await GameManager.Instance.GetAsteriy() >= 10)
		{
			GameManager.Instance.ChangeAsteriy(-10);
			GameManager.Instance.uiResourceShower.UpdateIndicators();
			int timeToRepair = (int)((1 - durability) * 100 / 2);
			StartCoroutine(Repair(timeToRepair, fixedBuilderRoom));
		}
	}

	private IEnumerator Repair(int time, GameObject room)
	{
		while (room.GetComponent<BuilderRoom>().fixedBear.GetComponent<UnitMovement>().currentRoutine != null)
		{
			yield return null;
		}
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", true);
		yield return new WaitForSeconds(time);
		room.GetComponent<BuilderRoom>().fixedBear.GetComponentInChildren<Animator>().SetBool("Work", false);
		durability = 1f;
	}

    public void SetStatus(Status status)
	{
		this.status = status;
	}

	public enum Resources
	{
		Energohoney,
		Asteriy,
		Cosmodrome,
		Bed,
		Build,
		Supply
	}

	public enum Status
	{
		Free,
		Busy,
		Destroyed
	}
}
