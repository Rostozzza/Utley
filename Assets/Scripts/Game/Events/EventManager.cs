
using UnityEngine.Events;
using static NotificationTypes;

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
	/// Triggered when energohoney room settings is opened;
	/// </summary>
	public static UnityEvent onEnergohoneySettingsOpened = new UnityEvent();

	/// <summary>
	/// Triggered when energohoney room settings were solved;
	/// </summary>
	public static UnityEvent onEnergohoneySettingsSolved = new UnityEvent();

	/// <summary>
	/// Triggered when player send off ursowaks
	/// </summary>
	public static UnityEvent onUrsovaxSent = new UnityEvent();

	/// <summary>
	/// Triggered when Supply room settings is opened;
	/// </summary>
	public static UnityEvent onSupplySettingsOpened = new UnityEvent();
	/// <summary>
	/// Triggered when player solves supply room settings;
	/// </summary>
	public static UnityEvent onSupplyRoomSettingsSolved = new UnityEvent();

	/// <summary>
	/// Triggered when any room got upgraded;
	/// </summary>
	public static UnityEvent onRoomUpgraded = new UnityEvent();

	/// <summary>
	/// Triggered when game ends, true - win, false = lose;
	/// </summary>
	public static UnityEvent<bool> onGameEnd = new UnityEvent<bool>();

	/// <summary>
	/// Triggered when method "ToMenu" calls in MenuManager
	/// </summary>
	public static UnityEvent onToMenuButton = new UnityEvent();

	#region Notification system
	/// <summary>
	/// Invoke error through this event. Pass error message into Invoke()
	/// </summary>
	public static UnityEvent<string> callError = new UnityEvent<string>();

	/// <summary>
	/// Invoke warning through this event. Pass warning message into Invoke()
	/// </summary>
	public static UnityEvent<string> callWarning = new UnityEvent<string>();

	/// <summary>
	/// Invoke standart notification through this event. Pass message into Invoke()
	/// </summary>
	public static UnityEvent<string> callMessage = new UnityEvent<string>();
	#endregion
}
