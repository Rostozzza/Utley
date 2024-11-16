using UnityEngine;

public class RoomCreator : MonoBehaviour
{
    [SerializeField] public GameObject mainRoom;
    public void CreateRoomAt()
    {
        GameObject room = Instantiate(mainRoom, Vector3.one, Quaternion.identity);
    }
}
