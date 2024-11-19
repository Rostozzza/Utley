using System.Collections.Generic;
using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using TMPro;

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
    [SerializeField] private Transform[] rawWalkPoints; // for energohoney 1 - 3 is paseka, 4 is generator
    private Vector3[] walkPoints;
    [SerializeField] private TextMeshProUGUI timeShow;
    private GameObject fixedBear;

    private void Start()
    {
        walkPoints = new Vector3[rawWalkPoints.Length];
        walkPoints = Array.ConvertAll(rawWalkPoints, obj => obj.position);
        leftDoor.SetActive(hasLeftDoor);
        rightDoor.SetActive(hasRightDoor);
        /*switch (resource)
        {
            case Resources.Energohoney:
                break;
        }*/
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
        fixedBear = bear;
        if (status == Status.Free)
        {
            work = StartCoroutine(WorkStatus());
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
        float timer;
        status = Status.Busy;
        fixedBear.GetComponent<UnitScript>().CannotBeSelected();
        switch (resource)
        {
            case Resources.Energohoney:
                fixedBear.GetComponent<UnitScript>().StartMoveInRoom((int)Resources.Energohoney);
                timer = 45f;
                while (timer > 0)
                {
                    timeShow.text = SecondsToTimeToShow(timer);
                    timer -= Time.deltaTime;
                    yield return null;
                }
                GameManager.Instance.ChangeHoney(10);
                break;
            case Resources.Asteriy:
                GameManager.Instance.ChangeAsteriy(10);
                break;
        }
        fixedBear.GetComponent<UnitScript>().CanBeSelected();
        fixedBear = null;
        status = Status.Free;
    }

    private string SecondsToTimeToShow(float seconds) // left - minutes, right - seconds. no hours
    {
        return (int)seconds / 60 + ":" + (((int)seconds % 60 < 10) ? "0" + (int)seconds % 60 : (int)seconds % 60);
    }

    public Vector3[] GetWalkPoints()
    {
        return walkPoints;
    }

    public enum Resources
    {
        Energohoney,
        Asteriy
    }

    public enum Status
    {
        Free,
        Busy,
        Destroyed
    }
}
