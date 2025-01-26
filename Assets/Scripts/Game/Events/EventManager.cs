
using UnityEngine.Events;

public static class EventManager
{
	public static UnityEvent bearSelected = new UnityEvent();
	/// <summary>
	/// When choosed any room from left list of rooms.
	/// </summary>
	public static UnityEvent onRoomSelected = new UnityEvent();
}
