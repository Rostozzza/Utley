using UnityEngine;

public class BuildRoomScript : MonoBehaviour
{
    [SerializeField] private GameObject elevator;
    public void BuildRoom(GameObject point)
    {
        GameManager.Instance.QueueBuildPos(point);
        GameManager.Instance.buildingScreen.SetActive(true);
    }
    public async void BuildElevator(GameObject point)
    {
		GameManager.Instance.QueueBuildPos(point);
        //GameManager.Instance.elevatorBuildingScreen.SetActive(true);
        GameManager.Instance.SelectAndBuild(elevator);
	}
}
