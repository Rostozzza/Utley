using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalEventInstance : MonoBehaviour
{
	private GlobalEvent model;
	private Coroutine eventRoutine;
	[SerializeField] private TextMeshProUGUI header;
	[SerializeField] private TextMeshProUGUI description;
	[SerializeField] private TextMeshProUGUI timeDescription;
	[SerializeField] private Image countdownImage;

	public void InitializeEventInstance(GlobalEvent model)
	{
		this.model = model;
		header.text = model.name;
		description.text = model.text;
		timeDescription.text = DateTime.Parse(model.start_date_time[..^1]) < DateTime.UtcNow ? "До начала:" : "До конца события:";
		Debug.Log((float)DateTime.Parse(model.start_date_time[..^1]).Subtract(DateTime.UtcNow.AddHours(+3)).TotalSeconds);
		eventRoutine = StartCoroutine(Countdown(DateTime.Parse(model.start_date_time[..^1]) > DateTime.UtcNow ? true : false));
	}

	private IEnumerator Countdown(bool isPreEvent)
	{
		Debug.Log("<color=yellow>Counddown");
		if (isPreEvent)
		{
			while (DateTime.Parse(model.start_date_time[..^1]) > DateTime.UtcNow.AddHours(3))
			{
				float currentTimeValue = (float)DateTime.Parse(model.start_date_time[..^1]).Subtract(DateTime.UtcNow.AddHours(+3)).TotalSeconds;
				timeDescription.text = $"До начала события: {SecondsToTimeToShow(currentTimeValue)}";
				countdownImage.fillAmount = currentTimeValue / 900f;
				yield return null;
			}
		}
		DateTime endTime = DateTime.Parse(model.start_date_time[..^1]).AddMinutes(model.duration_in_minutes);
		float waitFor = (float)endTime.Subtract(DateTime.UtcNow.AddHours(+3)).TotalSeconds;
		while (waitFor > 0)
		{
			waitFor = (float)endTime.Subtract(DateTime.UtcNow.AddHours(+3)).TotalSeconds;
			timeDescription.text = $"До конца события: {SecondsToTimeToShow(waitFor)}";
			countdownImage.fillAmount = waitFor / (model.duration_in_minutes*60f);
			yield return null;
		}
		try
		{
			MenuManager.Instance.GetEventTicker().KillActiveEvent(model);
		}
		catch
		{
			MenuManager.Instance.GetComponentInChildren<ModManager>().GetModEventsManager().KillActiveEvent(model);
		}
	}

	protected string SecondsToTimeToShow(float seconds) // left - minutes, right - seconds. no hours.
	{
		return (int)seconds / 60 + ":" + (((int)seconds % 60 < 10) ? "0" + (int)seconds % 60 : (int)seconds % 60);
	}
}