using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// God left us.
/// </summary>
public class SkillIssueManager : MonoBehaviour
{
	private Coroutine copeRoutine; 

	public void Update()
	{
		HandleResources();
		HandleSpeedTime();
		HandleRepairRooms();
	}

	/// <summary>
	/// time boost
	/// </summary>
	private void HandleSpeedTime() // time boost;
	{
		if (!Input.GetKeyDown(KeyCode.Keypad3) || copeRoutine != null)
		{
			return;
		}
		if (copeRoutine != null)
		{
			StopCoroutine(copeRoutine);
		}
		copeRoutine = StartCoroutine(SpeedTime(0.5f, 0, new List<KeyCode> { KeyCode.Keypad3, KeyCode.Keypad2, KeyCode.Keypad1 }));
	}

	/// <summary>
	/// resources
	/// </summary>
	private void HandleResources() // mnogo resursov;
	{
		if (!Input.GetKeyDown(KeyCode.Keypad1) || copeRoutine != null)
		{
			return;
		}
		if (copeRoutine != null)
		{
			StopCoroutine(copeRoutine);
		}
		copeRoutine = StartCoroutine(Resouces(0.5f, 0, new List<KeyCode> { KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad1 }));
	}

	/// <summary>
	/// unbreakable rooms
	/// </summary>
	private void HandleRepairRooms() // unbreakable rooms;
	{
		if (!Input.GetKeyDown(KeyCode.Keypad7) || copeRoutine != null)
		{
			return;
		}
		if (copeRoutine != null)
		{
			StopCoroutine(copeRoutine);
		}
		copeRoutine = StartCoroutine(RepairRooms(0.5f, 0, new List<KeyCode> { KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9 }));
	}

	private IEnumerator RepairRooms(float timeLeft, int now, List<KeyCode> killYourself)
	{
		while (timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
			if (Input.GetKeyDown(killYourself[now]))
			{
				if (now == killYourself.Count - 1)
				{
					GameManager.Instance.allRooms.Where(x => x.GetComponent<RoomScript>()).ToList().ForEach(x => x.GetComponent<RoomScript>().SetDurability(1) );
					Debug.Log("Repaired rooms");
					Debug.Log($"Breakage!: {now}");
					break;
				}
				yield return RepairRooms(timeLeft, now + 1, killYourself);
				Debug.Log($"Breakage!: {now}");
				break;
			}
			yield return null;
		}
		if (now == 1)
		{
			copeRoutine = null;
		}
		yield return null;
	}

	private IEnumerator Resouces(float timeLeft, int now, List<KeyCode> killYourself)
	{
		while (timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
			if (Input.GetKeyDown(killYourself[now]))
			{
				if (now == killYourself.Count - 1)
				{
					GameManager.Instance.ChangeAsteriy(100000,new Log());
					GameManager.Instance.ChangeAstroluminite(10000,new Log());
					GameManager.Instance.ChangeHNY(100000,new Log());
					GameManager.Instance.ChangePrototype(100000000,new Log());
					GameManager.Instance.ChangeUrsowaks(10000000000, new Log());
					GameManager.Instance.ChangeHoney(10000000000, new Log());
					Debug.Log("Resources");
					Debug.Log($"Breakage!: {now}");
					break;
				}
				yield return Resouces(timeLeft, now + 1, killYourself);
				Debug.Log($"Breakage!: {now}");
				break;
			}
			yield return null;
		}
		if (now == 1)
		{
			copeRoutine = null;
		}
		yield return null;
	}

	private IEnumerator SpeedTime(float timeLeft, int now, List<KeyCode> killYourself)
	{
		while (timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
			if (Input.GetKeyDown(killYourself[now]))
			{
				if (now == killYourself.Count - 1)
				{
					Time.timeScale = Time.timeScale == 2f ? 1f : 2f;
					Debug.Log($"Timescale: {Time.timeScale}");
					Debug.Log($"Breakage!: {now}");
					break;
				}
				yield return SpeedTime(timeLeft, now + 1, killYourself);
				Debug.Log($"Breakage!: {now}");
				break;
			}
			yield return null;
		}
		if (now == 1)
		{
			copeRoutine = null;
		}
		yield return null;
	}
}
