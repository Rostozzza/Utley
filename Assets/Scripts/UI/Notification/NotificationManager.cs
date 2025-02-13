using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using static NotificationTypes;
using UnityEngine.Events;

public class NotificationsManager : MonoBehaviour
{
	[Header("Notification draw settings")]
	[SerializeField] private GameObject prefab;
	[SerializeField] private Transform notificationGrid;

	/// <summary>
	/// Use this to set up new notification. Pass presets presented in "NotificationTypes" class into "type" field.
	/// </summary>
	/// <param name="type"></param>
	public UnityAction CreateNotification(string message, NotificationType type)
	{
		var notification = Instantiate(prefab, notificationGrid);
		notification.GetComponent<Notification>().InitializeNotification(type, message);
		return null;
	}

	public void Start()
	{
		EventManager.callError.AddListener(error => { CreateNotification(error, NotificationTypes.error); });
		EventManager.callWarning.AddListener(warning => { CreateNotification(warning, NotificationTypes.warning); });
		EventManager.callMessage.AddListener(message => { CreateNotification(message, NotificationTypes.message); });
	}
}
