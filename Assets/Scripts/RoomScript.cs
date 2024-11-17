using System.Collections.Generic;
using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [SerializeField] public Status status;
    [SerializeField] public Resources resourse;
    [SerializeField] public GameObject leftDoor;
    [SerializeField] public GameObject rightDoor;
    [SerializeField] public bool hasLeftDoor;
    [SerializeField] public bool hasRightDoor;
    public List<Elevator> connectedElevators;
    private Coroutine work;

    private void Start()
    {
        leftDoor.SetActive(hasLeftDoor);
        rightDoor.SetActive(hasRightDoor);
    }

    public void BuildRoom(GameObject button)
    {
        GameManager.Instance.QueueBuildPos(button);
    }


    /// <summary>
    /// Start work at station ( calls coroutine, can be interrupted by InterruptWork() )
    /// </summary>
    public void StartWork()
    {
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
    }

    private IEnumerator WorkStatus()
    {
        status = Status.Busy;
        yield return new WaitForSeconds(5f);
        switch (resourse)
        {
            case Resources.Energohoney:
                GameManager.Instance.ChangeHoney(10);
                break;
            case Resources.Asteriy:
                GameManager.Instance.ChangeAsteriy(10);
                break;
        }
        status = Status.Free;
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
