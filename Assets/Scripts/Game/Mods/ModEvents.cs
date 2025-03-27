
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using static TutorialManager;
using System.IO;

public class ModEvents : MonoBehaviour
{
	private Coroutine modTickerRoutione;
	[SerializeField] private float tickRate = 20f;
	private List<GlobalEvent> activeEvents;
	private List<LocalEvent> previewEvents;
	public List<LocalEvent> localEvents;
	private List<GameObject> eventViews = new List<GameObject>();
	[SerializeField] private GameObject eventPrefab;
	[SerializeField] private Transform eventsParent;


	public void KillActiveEvent(GlobalEvent globalEvent)
	{
		int ind = activeEvents.IndexOf(globalEvent);
		Debug.Log($"<color=yellow>{ind}");
		DropMultipliers(previewEvents[ind]);
		Destroy(eventViews[ind]);
		eventViews.RemoveAt(ind);
		if (activeEvents.Contains(globalEvent)) activeEvents.Remove(globalEvent);
	}

	public void ResetTicker()
	{
		activeEvents = new List<GlobalEvent>();
		previewEvents = new List<LocalEvent>();
		localEvents = new List<LocalEvent>();
		eventViews = new List<GameObject>();
		if (modTickerRoutione != null) StopCoroutine(modTickerRoutione);
	}

	public void StartTicker()
	{
		if (modTickerRoutione != null) StopCoroutine(modTickerRoutione);
		modTickerRoutione = StartCoroutine(Ticker());
	}

	private IEnumerator Ticker()
	{
		while (SceneManager.GetActiveScene().buildIndex == 0)
		{
			while (!eventsParent.gameObject.activeInHierarchy) yield return null;
			Debug.Log("<color=green>TICKER");
			TryAddModEvents();
			yield return new WaitForSeconds(tickRate);
			Debug.Log("Tick");
		}
	}

	public void TryAddModEvents()
	{
		Debug.Log(localEvents.Count);
		Debug.Log("Sosal");
		foreach (var newEvent in localEvents)
		{
			DateTime eventTime = DateTime.Parse(newEvent.start_date_time[..^1]);
			float timeBetween = (float)eventTime.Subtract(DateTime.UtcNow.AddHours(3)).TotalMinutes;
			Debug.Log(DateTime.UtcNow.AddHours(3));
			GlobalEvent model = new GlobalEvent
			{
				name = newEvent.name,
				text = newEvent.description,
				duration_in_minutes = newEvent.duration_in_minutes,
				start_date_time = newEvent.start_date_time
			};
			if (timeBetween <= 15 && timeBetween > -newEvent.duration_in_minutes && activeEvents.FirstOrDefault(x => x.name == newEvent.name) == null && previewEvents.FirstOrDefault(x => x.Equals(newEvent)) == null)
			{
				var newEventInstance = Instantiate(eventPrefab, eventsParent);
				eventViews.Add(newEventInstance);
				previewEvents.Add(newEvent);
				newEventInstance.GetComponent<GlobalEventInstance>().InitializeEventInstance(model);

				Texture2D SpriteTexture = new Texture2D(2, 2);
				SpriteTexture.LoadImage(File.ReadAllBytes(newEvent.icon_path));
				Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0));

				newEventInstance.GetComponentsInChildren<Image>()[1].sprite = NewSprite;
				EventManager.callWarning.Invoke(newEvent.name);
			}
			if (timeBetween <= 0 && activeEvents.FirstOrDefault(x => x.name == newEvent.name) == null && timeBetween > -newEvent.duration_in_minutes)
			{
				Debug.Log($"<color=red>{timeBetween}");
				//if (previewEvents.Contains(model)) previewEvents.Remove(model);
				activeEvents.Add(model);
				ApplyMultipliers(newEvent);
				Debug.Log("EVENT ACTIVE");
			}
		}
	}

	private void ApplyMultipliers(LocalEvent model)
	{
		var multipliers = model.multipliers;

		foreach (var multiplier in multipliers)
		{
			Debug.Log($"<color=green>MULTIPLIER {multiplier.Key} = {multiplier.Value}");
			switch (multiplier.Key)
			{
				case "EnergohoneyGain":
					Debug.Log(ValuesHolder.EnergohoneyAmountByOneInteraction);
					ValuesHolder.EnergohoneyAmountByOneInteraction *= multiplier.Value;
					Debug.Log(ValuesHolder.EnergohoneyAmountByOneInteraction);
					break;
				case "AsteriumGain":
					ValuesHolder.AsteriumAmountByOneInteraction = (int)(ValuesHolder.AsteriumAmountByOneInteraction * multiplier.Value);
					break;
				case "AstroluminiteGain":
					ValuesHolder.AstroluminiteAmountByOneInteraction = (int)(ValuesHolder.AstroluminiteAmountByOneInteraction * multiplier.Value);
					break;
				case "UrsowaksGain":
					ValuesHolder.UrsowaksAmountByOneInteraction = (int)(ValuesHolder.UrsowaksAmountByOneInteraction * multiplier.Value);
					break;
				case "PrototypeGain":
					ValuesHolder.PrototypeAmountByOneInteraction = (int)(ValuesHolder.PrototypeAmountByOneInteraction * multiplier.Value);
					break;
				case "EnergohoneyConsumeMultiplier":
					ValuesHolder.EnergohoneyConsumeMultiplier = (ValuesHolder.EnergohoneyConsumeMultiplier * multiplier.Value);
					break;
				case "DamageByTide":
					ValuesHolder.DamageByTide = (ValuesHolder.DamageByTide * multiplier.Value);
					break;
			}
		}
	}
	private void DropMultipliers(LocalEvent model)
	{
		var multipliers = model.multipliers;

		foreach (var multiplier in multipliers)
		{
			Debug.Log($"<color=green>MULTIPLIER {multiplier.Key} = {multiplier.Value}");
			switch (multiplier.Key)
			{
				case "EnergohoneyGain":
					Debug.Log(ValuesHolder.EnergohoneyAmountByOneInteraction);
					ValuesHolder.EnergohoneyAmountByOneInteraction /= multiplier.Value;
					Debug.Log(ValuesHolder.EnergohoneyAmountByOneInteraction);
					break;
				case "AsteriumGain":
					ValuesHolder.AsteriumAmountByOneInteraction = (int)(ValuesHolder.AsteriumAmountByOneInteraction / multiplier.Value) ;
					break;
				case "AstroluminiteGain":
					ValuesHolder.AstroluminiteAmountByOneInteraction = (int)(ValuesHolder.AstroluminiteAmountByOneInteraction / multiplier.Value);
					break;
				case "UrsowaksGain":
					ValuesHolder.UrsowaksAmountByOneInteraction = (int)(ValuesHolder.UrsowaksAmountByOneInteraction / multiplier.Value);
					break;
				case "PrototypeGain":
					ValuesHolder.PrototypeAmountByOneInteraction = (int)(ValuesHolder.PrototypeAmountByOneInteraction / multiplier.Value);
					break;
				case "EnergohoneyConsumeMultiplier":
					ValuesHolder.EnergohoneyConsumeMultiplier = (ValuesHolder.EnergohoneyConsumeMultiplier / multiplier.Value);
					break;
				case "DamageByTide":
					ValuesHolder.DamageByTide = (ValuesHolder.DamageByTide / multiplier.Value);
					break;
			}
		}
	}
}
