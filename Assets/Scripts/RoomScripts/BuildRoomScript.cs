using UnityEngine;

public class BuildRoomScript : MonoBehaviour
{
    public void BuildRoom(GameObject point)
    {
        GameManager.Instance.QueueBuildPos(point);
    }
}
