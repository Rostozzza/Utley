using System.Collections.Generic;
using System.Linq;
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
      
      try
      {
        GameObject.FindGameObjectsWithTag("UI_canvas").ToList().ForEach(x => x.GetComponent<Canvas>().worldCamera = Camera.main.GetComponentsInChildren<Camera>()[1]);
      }
      catch
      {
        GameObject.FindGameObjectsWithTag("UI_canvas").ToList().ForEach(x => x.GetComponent<Canvas>().worldCamera = Camera.main);
      }
    }

    public void BuildElevator(GameObject point)
    {
        GameManager.Instance.QueueBuildPos(point);
    }
}
