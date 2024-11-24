using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;

public class RoomScript : MonoBehaviour
{
	[SerializeField] public Status status;
	[SerializeField] public Resources resource;
	[SerializeField] public GameObject leftDoor;
	[SerializeField] public GameObject rightDoor;
	[SerializeField] public bool hasLeftDoor;
	[SerializeField] public bool hasRightDoor;
	[SerializeField] private GameObject roomStatsScreen;

	public List<Elevator> connectedElevators;
	public List<RoomScript> connectedRooms;
	private Coroutine work;
	//[SerializeField] private Transform[] rawWalkPoints; // for energohoney 1 - 3 is paseka, 4 is generator
	[SerializeField] private List<Transform> rawWalkPoints;
	//private Vector3[] walkPoints;
	private List<Vector3> walkPoints;
	[SerializeField] private TextMeshProUGUI timeShow;
	[SerializeField] private GameObject fixedBear;
	[SerializeField] private List<GameObject> workStationsToOutline;
	[SerializeField] public float durability = 1f;
	[SerializeField] public int level = 1;
	[SerializeField] public int depthLevel;
	[Header("Asterium settings")]
	public bool isReadyForWork = false;

	private void Start()
	{
		walkPoints = rawWalkPoints.ConvertAll(n => n.transform.position);
		roomStatsScreen = transform.Find("RoomInfo").gameObject;
		roomStatsScreen.SetActive(false);
		depthLevel = (int)((2 - transform.position.y) / 4);
		switch (resource)
		{
			case Resources.Energohoney:
				GameManager.Instance.AddWorkStations(workStationsToOutline);
				break;
			case Resources.Cosmodrome:
				GameManager.Instance.AddWorkStations(workStationsToOutline);
				break;
			case Resources.Bed:
				GameManager.Instance.AddWorkStations(workStationsToOutline);
				GameManager.Instance.ChangeMaxBearAmount(6);
				break;
			case Resources.Build:
				GameManager.Instance.AddWorkStations(workStationsToOutline);
				break;
		}
	}

	public void UpgradeRoom(GameObject button)
	{
		if (GameManager.Instance.GetHoney() <= (30 + 10 * (level - 1)))
		{
			GameManager.Instance.ChangeHoney(-(30 + 10 * (level - 1)));
			level += 1;
			UpdateRoomHullView();
			GameManager.Instance.uiResourceShower.UpdateIndicators();
			if (level == 3)
			{
				button.GetComponent<Button>().enabled = false;
				button.GetComponentInChildren<TextMeshProUGUI>().text = "Максимальный уровень!";
			}
		}
	}

	public void ToggleRoomStats(bool toggle)
	{
		roomStatsScreen.SetActive(toggle);
		roomStatsScreen.transform.Find("Level (1)").GetComponent<TextMeshProUGUI>().text = "";
		for (int i = 0; i < level; i++)
		{
			roomStatsScreen.transform.Find("Level (1)").GetComponent<TextMeshProUGUI>().text += "I";
		}
		UpdateRoomHullView();
	}

	public void UpdateRoomHullView()
	{
		roomStatsScreen.transform.Find("hull%").GetComponent<TextMeshProUGUI>().text = $"{Mathf.RoundToInt((durability / 1f) * 100f)}%";
		roomStatsScreen.transform.Find("Hull").localScale = new Vector3(durability/1f,1,1);
	}

	public void BuildRoom(GameObject button)
	{
		GameManager.Instance.QueueBuildPos(button);
		GameManager.Instance.buildingScreen.SetActive(true);
	}

	/// <summary>
	/// Start work at station by bear ( calls coroutine, can be interrupted by InterruptWork() )
	/// </summary>
	/// <param name="bear"></param>
	public void StartWork(GameObject bear)
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
			}
			fixedBear = bear;
			if (resource == Resources.Cosmodrome)
			{
				status = Status.Busy;
				fixedBear.GetComponent<UnitScript>().CannotBeSelected();
				timeShow.transform.parent.gameObject.SetActive(true);
				return;
			}
			if (status == Status.Free)
			{
				work = StartCoroutine(WorkStatus());
			}
		}
	}

	public void StartCosmodromeWork()
	{
		if (GameManager.Instance.FlyForRawAsterium())
		{
			timeShow.gameObject.SetActive(true);
			StartCoroutine(WorkStatus());
		}
	}

	/// <summary>
	/// Stops work
	/// </summary>
	public void InterruptWork()
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

	private IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		if (resource != Resources.Asteriy)
		{
			fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		}
		switch (resource)
		{
			case Resources.Energohoney:
				fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Energohoney, GetWalkPoints(), this.gameObject);
				if (fixedBear.GetComponent<UnitScript>().job == Qualification.beekeeper)
				{
					timer = 45f *(1-0.25f*(level-1)) * (1 - (Mathf.FloorToInt(fixedBear.GetComponent<UnitScript>().level) * 0.5f));
				}
				else
				{
					timer = 45f * 1.25f * (1 - 0.25f * (level - 1));
				}
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
				timeShow.text = "";
				
				int honeyToAdd = (GameManager.Instance.season != GameManager.Season.Storm) ? 10 : (int)(10 * (1 - 0.15f + 0.03f * GameManager.Instance.cycleNumber));
				GameManager.Instance.ChangeHoney(honeyToAdd);
				GameManager.Instance.uiResourceShower.UpdateIndicators();
				if (fixedBear.GetComponent<UnitScript>().job == Qualification.beekeeper)
				{
					fixedBear.GetComponent<UnitScript>().LevelUpBear();
				}
				break;
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
				break;
			case Resources.Bed:
				if (fixedBear.GetComponent<UnitScript>().job == Qualification.creator)
				{
					timer = 45f * (1 - 0.25f * (level - 1)) * (1 - (Mathf.FloorToInt(fixedBear.GetComponent<UnitScript>().level) * 0.5f));
				}
				else
				{
					timer = 45f * 1.25f * (1 - 0.25f * (level - 1));
				}
				fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Bed, GetWalkPoints(), this.gameObject);
				timer = 120f;
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
				if (fixedBear.GetComponent<UnitScript>().job == Qualification.creator)
				{
					fixedBear.GetComponent<UnitScript>().LevelUpBear();
				}
				timeShow.text = "";
				GameManager.Instance.BoostThreeBears();
				break;
		}
		if (fixedBear != null)
		{
			fixedBear.GetComponent<UnitScript>().CanBeSelected();
			fixedBear = null;
		}
		status = Status.Free;
	}

	private string SecondsToTimeToShow(float seconds) // left - minutes, right - seconds. no hours.
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
		durability += hp;
		if (durability <= 0)
		{
			status = Status.Destroyed;
		}
		durability = Mathf.Clamp(durability, 0f, 1f);
	}

    /// <summary>
	/// Repairs room to full for 10 asterium
	/// </summary>
	public void RepairRoom()
	{
		if (GameManager.Instance.GetAsteriy() >= 10)
		{
			GameManager.Instance.ChangeAsteriy(-10);
			int timeToRepair = (int)((1 - durability) * 100 / 2);
			StartCoroutine(Repair(timeToRepair));
		}
		GameManager.Instance.uiResourceShower.UpdateIndicators();
	}

	private IEnumerator Repair(int seconds)
	{
		yield return new WaitForSeconds(seconds);
		durability = 1f;
	}

	public enum Resources
	{
		Energohoney,
		Asteriy,
		Cosmodrome,
		Bed,
		Build
	}

	public enum Status
	{
		Free,
		Busy,
		Destroyed
	}
}
