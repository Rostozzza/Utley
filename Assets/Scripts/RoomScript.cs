using System.Collections.Generic;
using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [SerializeField] public Status status;
    [SerializeField] public string resourse;
    [SerializeField] public GameObject leftDoor;
    [SerializeField] public GameObject rightDoor;
    [SerializeField] public bool hasLeftDoor;
    [SerializeField] public bool hasRightDoor;
    public List<Elevator> connectedElevators;
    public List<RoomScript> connectedRooms;
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
    /// Начать работу за станцией
    /// </summary>
    public void StartWork()
    {
        if (status == Status.Free)
        {
            work = StartCoroutine(WorkStatus());
        }
    }

    public void InterruptWork()
    {
        StopCoroutine(work);
    }

    private IEnumerator WorkStatus()
    {
        status = Status.Busy;
        yield return new WaitForSeconds(5f);
        status = Status.Free;
    }

    public enum Status
    {
        Free,
        Busy,
        Destroyed
    }
}
