
using UnityEngine.Events;

public static class EventManager
{
	/// <summary> 
	/// Triggered when player selects any unit;
	/// </summary>
	public static UnityEvent onBearSelected = new UnityEvent();

	/// <summary>
	/// When chose any room from left list of rooms;
	/// </summary>
	public static UnityEvent onRoomSelected = new UnityEvent();

	/// <summary>
	/// Triggered when unit reaches its destination room;
	/// </summary>
	public static UnityEvent<RoomScript> onBearReachedDestination = new UnityEvent<RoomScript>();

	/// <summary>
	/// Triggered when unit starts their work with any room;
	/// </summary>
	public static UnityEvent<RoomScript> onBearWorkStarted = new UnityEvent<RoomScript>();

	/// <summary>
	/// Triggered when energohoney room settings is opened
	/// </summary>
	public static UnityEvent onEnergohoneySettingsOpened = new UnityEvent();
}
