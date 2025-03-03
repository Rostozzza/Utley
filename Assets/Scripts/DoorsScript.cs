using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DoorsScript : MonoBehaviour
{
    [SerializeField] public RoomType roomType;
	[SerializeField] private int index;
	[SerializeField] public GameObject leftDoor;
	[SerializeField] public GameObject rightDoor;
    [SerializeField] public GameObject topTrapDoor;
    [SerializeField] public GameObject bottomTrapDoor;
	[SerializeField] public bool hasLeftDoor;
	[SerializeField] public bool hasRightDoor;
    [SerializeField] public bool hasTopTrapDoor;
    [SerializeField] public bool hasBottomTrapDoor;
	
	[SerializeField] public bool useCustomSettings;
	[Header("Custom Settings")]
	[SerializeField] private float CenterOffsetX;
	[SerializeField] private float CenterOffsetY;
	[SerializeField] private float HorizontalRange;
	[SerializeField] private float VerticalRange;
	[SerializeField] private ForcedSetting ForceLeftDoor;
	[SerializeField] private ForcedSetting ForceRightDoor;
	[SerializeField] private ForcedSetting ForceTopDoor;
	[SerializeField] private ForcedSetting ForceBottomDoor;

    void Start()
    {
		CheckAndHideDoors(true);
		foreach (var room in GameManager.Instance.allRooms.Where(x => x.GetComponent<RoomScript>() && x!= gameObject))
		{
			room.GetComponent<DoorsScript>().CheckAndHideDoors(true);
		}
    }

	public void CheckAndHideDoors()
	{
		CheckAndHideDoors(false);
	}

	public void CheckAndHideDoors(bool checkNeighbours)
	{
		NearRooms nearRooms = new NearRooms();
        switch (roomType)
        {
            case RoomType.Room:
				nearRooms = CheckNearRooms(checkNeighbours);
                SetDoorsHide(nearRooms.leftRoom, nearRooms.rightRoom);
                break;
            case RoomType.Elevator:
				List<DoorsScript> group = new();
				if (TryGetComponent<Elevator>(out Elevator elevator))
				{
					//group.Add(this);
					group.AddRange(GetComponentsInChildren<DoorsScript>());
				}
				else
				{
					//group.Add(transform.parent.GetComponent<DoorsScript>());
					group.AddRange(transform.parent.GetComponentsInChildren<DoorsScript>());
				}
				group = group.OrderBy(x => x.transform.position.y).ToList();
				index = group.IndexOf(this);

				nearRooms = CheckNearRooms(checkNeighbours);
				nearRooms.topRoom = false;
				nearRooms.bottomRoom = false;
				if (group.IndexOf(this) == 0)
				{
					nearRooms.bottomRoom = true;
				}
				else if (group.IndexOf(this) == group.Count - 1)
				{
					nearRooms.topRoom = true;
				}
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
		bool HasRoomAt(Vector3 dir, float len)
		{
			List<Transform> hits;
			hits = Physics.RaycastAll(useCustomSettings ? transform.position + new Vector3(CenterOffsetX, CenterOffsetY) : transform.position, dir, len).ToList().ConvertAll(x => x.transform);
			foreach (Transform probablyRoom in hits)
			{
				if (probablyRoom.TryGetComponent(out DoorsScript doorsScript) && probablyRoom != gameObject)
				{
					if (checkNeighbours) doorsScript.Invoke(nameof(CheckAndHideDoors), 0.2f);
					return false;
				}
				//else if (probablyRoom.TryGetComponent(out BuildRoomScript elevatorScript))
				//{
				//	return false;
				//}
			}
			return true;
		}

		if (useCustomSettings)
		{
			NearRooms nearRooms = new
        	(
				HasRoomAt(Vector3.left, HorizontalRange / 2f),
				HasRoomAt(Vector3.right, HorizontalRange / 2f),
        	    HasRoomAt(Vector3.up, VerticalRange / 2f),
        	    HasRoomAt(Vector3.down, VerticalRange / 2f)
        	);
			return nearRooms;
		}
		else
		{
			NearRooms nearRooms = new
        	(
				HasRoomAt(Vector3.left, 8f),
				HasRoomAt(Vector3.right, 8f),
        	    HasRoomAt(Vector3.up, 4f),
        	    HasRoomAt(Vector3.down, 4f)
        	);
			return nearRooms;
		}
	}

	public void SetDoorsHide(bool LDoor, bool RDoor)
	{
		hasLeftDoor = LDoor;
		hasRightDoor = RDoor;
		leftDoor.SetActive(ForcedDoor(ForceLeftDoor, LDoor));
		rightDoor.SetActive(ForcedDoor(ForceRightDoor, RDoor));
	}

    public void SetDoorsHide(NearRooms nearRooms)
    {
		hasLeftDoor = nearRooms.leftRoom;
		hasRightDoor = nearRooms.rightRoom;
		hasTopTrapDoor = nearRooms.topRoom;
		hasBottomTrapDoor = nearRooms.bottomRoom;
		leftDoor.SetActive(ForcedDoor(ForceLeftDoor, nearRooms.leftRoom));
		rightDoor.SetActive(ForcedDoor(ForceRightDoor, nearRooms.rightRoom));
        topTrapDoor.SetActive(ForcedDoor(ForceTopDoor, nearRooms.topRoom));
        bottomTrapDoor.SetActive(ForcedDoor(ForceBottomDoor, nearRooms.bottomRoom));
    }

	private bool ForcedDoor(ForcedSetting forcedSetting, bool origin)
	{
		switch (forcedSetting)
		{
			case ForcedSetting.None:
				return origin;
			case ForcedSetting.Open:
				return false;
			case ForcedSetting.Close:
				return true;
			default:
				return origin;
		}
	}

    public enum RoomType
    {
        Room,
        Elevator
    }

	public enum ForcedSetting
	{
		None,
		Open,
		Close
	}
}
