using UnityEngine;

public class DoorWallScript : MonoBehaviour
{
    [SerializeField] bool door = false;
    [SerializeField] GameObject wallForDoor;

    private void Start()
    {
        if (door)
        {
            Instantiate(wallForDoor, new Vector3(0,-0.625f,-0.05f) + transform.position, Quaternion.identity, this.transform);
        }
    }
}
