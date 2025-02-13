
using System;
using UnityEngine;

public static class NotificationTypes
{
	[Serializable]
	public struct NotificationType
	{
		public string titleText;
		public float screenTime;
		public float shakeSpeed;
		public float shakeTime;
		public Color bgColor;
	}
	#region Notifications
	public static NotificationType warning = new NotificationType
	{
		titleText = "ВНИМАНИЕ",
		screenTime = 5f,
		shakeSpeed = 1.0f,
		shakeTime = 0.25f,
		bgColor = Color.red
	};
	public static NotificationType error = new NotificationType
	{
		titleText = "ОСТОРОЖНО",
		screenTime = 2.5f,
		shakeSpeed = 1.0f,
		shakeTime = 0.2f,
		bgColor = Color.red
	};
	public static NotificationType message = new NotificationType
	{
		screenTime = 2.5f,
		shakeSpeed = 0,
		shakeTime = 0,
		bgColor = Color.blue
	};
	#endregion
}
