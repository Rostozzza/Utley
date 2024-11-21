using System.Collections.Generic;
using System;
using System.Collections;
using UnityEditor.PackageManager;
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
	private GameObject fixedBear;
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
                if (!isReadyForWork)
                {
                    Debug.Log("нету астерия!");
                    return;
                }
                break;
        }
        Debug.Log("начали работу");
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
		if (status == Status.Free)
		{
			work = StartCoroutine(WorkStatus());
		}
	}

	public void StartCosmodromeWork()
	{
		if (GameManager.Instance.FlyForRawAsterium())
		{
			StartCoroutine(WorkStatus());
		}
	}

	/// <summary>
	/// Stops work
	/// </summary>
	public void InterruptWork()
	{
		StopCoroutine(work);
		work = null;
		fixedBear = null;
	}

	private IEnumerator WorkStatus()
	{
		Debug.Log("и даже корутину!" + fixedBear);
		float timer;
		status = Status.Busy;
		fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		switch (resource)
		{
			case Resources.Energohoney:
				fixedBear.GetComponent<UnitScript>().StartMoveInRoom((int)Resources.Energohoney, GetWalkPoints());
				timer = 45f;
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
				GameManager.Instance.WithdrawRawAsterium();
				GameManager.Instance.ChangeAsteriy(10);
				isReadyForWork = false;
				GameManager.Instance.uiResourceShower.UpdateIndicators();
				break;
			case Resources.Cosmodrome:
				timer = 45f;
				while (timer > 0)
				{
					timeShow.text = SecondsToTimeToShow(timer);
					timer -= Time.deltaTime;
					yield return null;
				}
				GameManager.Instance.DeliverRawAsterium();
				GameManager.Instance.uiResourceShower.UpdateIndicators();
				break;
		}
		fixedBear.GetComponent<UnitScript>().CanBeSelected();
		fixedBear = null;
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
