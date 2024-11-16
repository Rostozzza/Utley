using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [SerializeField] public Status status;
    [SerializeField] public string resourse;
    private Coroutine work;

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
