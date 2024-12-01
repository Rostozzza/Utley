using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Elevator : MonoBehaviour
{
    public List<Elevator> connectedElevators;
    public List<RoomScript> connectedRooms;
    public ElevatorModel elevatorModel;

    private void Start()
    {
		foreach (var button in GetComponentsInChildren<Button>())
		{
			button.gameObject.SetActive(GameManager.Instance.mode == GameManager.Mode.Build);
		}
    }

    public void BuildElevator(GameObject point)
    {
        GameManager.Instance.QueueBuildPos(point);
    }
}
