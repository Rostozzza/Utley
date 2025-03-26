using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using static NotificationTypes;
using UnityEngine.Events;
using System.Linq;
using TMPro;

public class NotificationsManager : MonoBehaviour
{
	[Header("Notification draw settings")]
	[SerializeField] private GameObject prefab;
	[SerializeField] private Transform notificationGrid;
	[Space]
	static public NotificationsManager Instance;
	[SerializeField] private List<Notification> activeNotificationTexts;

    void Awake()
    {
        if (Instance == null)
		{
			Instance = this;
		}
    }

    /// <summary>
    /// Use this to set up new notification. Pass presets presented in "NotificationTypes" class into "type" field.
    /// </summary>
    /// <param name="type"></param>
    public UnityAction CreateNotification(string message, NotificationType type)
	{
		if (activeNotificationTexts.Count > 0) return null;

		var notification = Instantiate(prefab, notificationGrid);
		notification.GetComponent<Notification>().InitializeNotification(type, message);
		return null;
	}
	public UnityAction CreateNotification(GlobalEvent globalEvent, NotificationType type)
	{
		if (activeNotificationTexts.Count > 0) return null;

		var notification = Instantiate(prefab, notificationGrid);
		notification.GetComponentInChildren<TextMeshProUGUI>().text = globalEvent.name;
		notification.GetComponent<Notification>().InitializeNotification(type, globalEvent.text);
		return null;
	}

	public void Start()
	{
		EventManager.callError.AddListener(error => { CreateNotification(error, NotificationTypes.error); });
		EventManager.callWarning.AddListener(warning => { CreateNotification(warning, NotificationTypes.warning); });
		EventManager.callMessage.AddListener(message => { CreateNotification(message, NotificationTypes.message); });
		EventManager.callGlobalEventNotification.AddListener(globalEvent => { CreateNotification(globalEvent, NotificationTypes.globalEvent); });
	}

	public void AddActiveNotificationText(Notification notification) => activeNotificationTexts.Add(notification);
	public void RemoveActiveNotificationText(Notification notification) => activeNotificationTexts.Remove(notification);
}
