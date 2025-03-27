using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class GlobalEventTicker : MonoBehaviour
{
	private Coroutine tickerRoutione;
	[SerializeField] private float tickRate = 120f;
	private List<GlobalEvent> activeEvents;
	[SerializeField] private GameObject eventPrefab;
	[SerializeField] private Transform eventsParent;

	public void KillActiveEvent(GlobalEvent globalEvent)
	{
		if (activeEvents.Contains(globalEvent)) activeEvents.Remove(globalEvent);
	}

	public void StartTicker()
	{
		if (tickerRoutione != null) StopCoroutine(tickerRoutione);
		tickerRoutione = StartCoroutine(Ticker());
	}

	private IEnumerator Ticker()
	{
		yield return RecieveGlobalEvents();
		while (MenuManager.Instance.isAPIActive)
		{
			yield return new WaitForSeconds(tickRate);
			yield return RecieveGlobalEvents();
		}
	}

	public void TryAddModEvent(LocalEvent newEvent)
	{
		DateTime eventTime = DateTime.Parse(newEvent.start_date_time[..^1]);
		int timeBetween = (int)eventTime.Subtract(DateTime.UtcNow.AddHours(3)).TotalMinutes;
		Debug.Log(DateTime.UtcNow.AddHours(3));
		GlobalEvent model = new GlobalEvent {
			name = newEvent.name,
			text = newEvent.description,
			duration_in_minutes = newEvent.duration_in_minutes,
			start_date_time = newEvent.start_date_time
		};
		if (timeBetween <= 15)
		{
			var newEventInstance = Instantiate(eventPrefab, eventsParent);
			newEventInstance.GetComponent<GlobalEventInstance>().InitializeEventInstance(model);
			EventManager.callWarning.Invoke(newEvent.name);
		}
		if (timeBetween <= 0)
		{
			activeEvents.Add(model);
			Debug.Log("EVENT ACTIVE");
		}
	}

	private async Task RecieveGlobalEvents()
	{
		List<GlobalEvent> list = await MenuManager.Instance.RequestManager.GetAllGlobalEvents();
		if (list == null || list.Count == 0)
		{
			//Take info from saved data (if present);
			return;
		}
		activeEvents = new List<GlobalEvent>();
		foreach (var globalEvent in list)
		{
			DateTime eventTime = DateTime.Parse(globalEvent.start_date_time[..^1]);
			int timeBetween = (int)eventTime.Subtract(DateTime.UtcNow.AddHours(3)).TotalMinutes;
			Debug.Log(DateTime.UtcNow.AddHours(3));
			if (timeBetween <= 15)
			{
				var newEventInstance = Instantiate(eventPrefab, eventsParent);
				newEventInstance.GetComponent<GlobalEventInstance>().InitializeEventInstance(globalEvent);
				EventManager.callWarning.Invoke(globalEvent.name);
			}
			if (timeBetween <= 0)
			{
				activeEvents.Add(globalEvent);

				Debug.Log("EVENT ACTIVE");
			}
		}
	}
}