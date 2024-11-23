using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomScript : MonoBehaviour
{
	[SerializeField] public Status status;
	[SerializeField] public Resources resource;
	[SerializeField] public GameObject leftDoor;
	[SerializeField] public GameObject rightDoor;
	[SerializeField] public bool hasLeftDoor;
	[SerializeField] public bool hasRightDoor;

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
	[Header("Asterium settings")]
	public bool isReadyForWork = false;

	private void Start()
	{
		//walkPoints = new Vector3[rawWalkPoints.Length];
		//walkPoints = Array.ConvertAll(rawWalkPoints, obj => obj.position);
		walkPoints = rawWalkPoints.ConvertAll(n => n.transform.position);
		//leftDoor.SetActive(hasLeftDoor);
		//rightDoor.SetActive(hasRightDoor);
		switch (resource)
		{
			case Resources.Energohoney:
				GameManager.Instance.AddWorkStations(workStationsToOutline);
				break;
			case Resources.Cosmodrome:
				GameManager.Instance.AddWorkStations(workStationsToOutline);
				break;
		}
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
		switch (resource)
		{
			case Resources.Energohoney:
				break;
			case Resources.Asteriy:
				work = StartCoroutine(WorkStatus());
				return;
		}
		if (resource == Resources.Asteriy)
		{
			work = StartCoroutine(WorkStatus());
			return;
		}
		fixedBear = bear;
		/*if (resource == Resources.Asteriy)
		{
            if (!isReadyForWork)
			{
				Debug.Log("нету астерия!");
				return;
			}
			Debug.Log("начали работу");
			fixedBear = bear;
		}*/
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
				fixedBear.GetComponent<UnitScript>().StartMoveInRoom((int)Resources.Energohoney, GetWalkPoints(), this.gameObject);
				if (fixedBear.GetComponent<UnitScript>().job == Qualification.beekeeper)
				{
					timer = 45f * (1 - (fixedBear.GetComponent<UnitScript>().level * 0.5f));
				}
				else
				{
					timer = 45f * 1.25f;
				}
				while (timer > 0)
				{
					timeShow.text = SecondsToTimeToShow(timer);
					timer -= Time.deltaTime;
					yield return null;
				}
				timeShow.text = "";
				GameManager.Instance.ChangeHoney(10);
				GameManager.Instance.uiResourceShower.UpdateIndicators();
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
	public enum Resources
	{
		Energohoney,
		Asteriy,
		Cosmodrome
	}

	public enum Status
	{
		Free,
		Busy,
		Destroyed
	}
}
