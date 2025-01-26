
using UnityEngine.Events;

public static class EventManager
{
	/// <summary> 
	/// Triggered when player selects any unit;
	/// </summary>
	public static UnityEvent onBearSelected = new UnityEvent();
	/// <summary>
	/// When choosed any room from left list of rooms;
	/// </summary>
	public static UnityEvent onRoomSelected = new UnityEvent();

	/// <summary>
	/// Triggered when unit reaches its destination room;
	/// </summary>
	public static UnityEvent<RoomScript> onBearReachedDestination = new UnityEvent<RoomScript>();
}
