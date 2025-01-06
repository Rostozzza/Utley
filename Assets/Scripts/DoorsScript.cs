using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DoorsScript : MonoBehaviour
{
    [SerializeField] public RoomType roomType;
	[SerializeField] public GameObject leftDoor;
	[SerializeField] public GameObject rightDoor;
    [SerializeField] public GameObject topTrapDoor;
    [SerializeField] public GameObject bottomTrapDoor;
	[SerializeField] public bool hasLeftDoor;
	[SerializeField] public bool hasRightDoor;
    [SerializeField] public bool hasTopTrapDoor;
    [SerializeField] public bool hasBottomTrapDoor;

    void Start()
    {
		CheckAndHideDoors(true);
    }

	public void CheckAndHideDoors(bool checkNeighbours)
	{
		NearRooms nearRooms = CheckNearRooms(checkNeighbours);
        switch (roomType)
        {
            case RoomType.Room:
                SetDoorsHide(nearRooms.leftRoom, nearRooms.rightRoom);
                break;
            case RoomType.Elevator:
                SetDoorsHide(nearRooms);
                break;
        }
		
		//Debug.Log(name + " ДВЕРИ: " + nearRooms.leftRoom + " " + nearRooms.rightRoom);
	}

	public class NearRooms
	{
		public  bool leftRoom { get; set; }
		public  bool rightRoom { get; set; }
		public  bool topRoom { get; set; }
		public  bool bottomRoom { get; set; }
		public NearRooms() { leftRoom = false; rightRoom = false; topRoom = false; bottomRoom = false; }
		public NearRooms(bool leftRoom, bool rightRoom, bool topRoom, bool bottomRoom)
		{
			this.leftRoom = leftRoom;
			this.rightRoom = rightRoom;
			this.topRoom = topRoom;
			this.bottomRoom = bottomRoom;
		}
		public NearRooms(bool leftRoom, bool rightRoom) { this.leftRoom = leftRoom; this.rightRoom = rightRoom; topRoom = false; bottomRoom = false; }
	}

	public NearRooms CheckNearRooms(bool checkNeighbours)
	{
		bool HasRoomAt(Vector3 dir)
		{
			List<Transform> hits;
			hits = Physics.RaycastAll(transform.position, dir, (dir.y == 0) ? 8f : 4f).ToList().ConvertAll(x => x.transform);
			foreach (Transform probablyRoom in hits)
			{
				if (probablyRoom.TryGetComponent(out DoorsScript doorsScript))
				{
					if (checkNeighbours) doorsScript.CheckAndHideDoors(false);
					return false;
				}
				else if (probablyRoom.TryGetComponent(out BuildRoomScript elevatorScript))
				{
					return false;
				}
			}
			return true;
		}

		NearRooms nearRooms = new
        (
			HasRoomAt(Vector3.left),
			HasRoomAt(Vector3.right),
            HasRoomAt(Vector3.up),
            HasRoomAt(Vector3.down)
        );

		return nearRooms;
	}

	public void SetDoorsHide(bool LDoor, bool RDoor)
	{
		hasLeftDoor = LDoor;
		hasRightDoor = RDoor;
		leftDoor.SetActive(LDoor);
		rightDoor.SetActive(RDoor);
	}

    public void SetDoorsHide(NearRooms nearRooms)
    {
		hasLeftDoor = nearRooms.leftRoom;
		hasRightDoor = nearRooms.rightRoom;
		hasTopTrapDoor = nearRooms.topRoom;
		hasBottomTrapDoor = nearRooms.bottomRoom;
		leftDoor.SetActive(nearRooms.leftRoom);
		rightDoor.SetActive(nearRooms.rightRoom);
        topTrapDoor.SetActive(nearRooms.topRoom);
        bottomTrapDoor.SetActive(nearRooms.bottomRoom);
    }

    public enum RoomType
    {
        Room,
        Elevator
    }
}
