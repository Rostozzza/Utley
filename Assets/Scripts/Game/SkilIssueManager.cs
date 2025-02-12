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
		HandleSosi();
		HandleGoida();
		HandleHui();
	}

	/// <summary>
	/// L + Ratio
	/// </summary>
	private void HandleGoida()
	{
		if (!Input.GetKeyDown(KeyCode.Z) || copeRoutine != null)
		{
			return;
		}
		if (copeRoutine != null)
		{
			StopCoroutine(copeRoutine);
		}
		copeRoutine = StartCoroutine(Goida(0.5f, 0, new List<KeyCode> { KeyCode.Z, KeyCode.O, KeyCode.V }));
	}

	/// <summary>
	/// Cope.
	/// </summary>
	private void HandleSosi()
	{
		if (!Input.GetKeyDown(KeyCode.S) || copeRoutine != null)
		{
			return;
		}
		if (copeRoutine != null)
		{
			StopCoroutine(copeRoutine);
		}
		copeRoutine = StartCoroutine(Sosi(0.5f, 0, new List<KeyCode> { KeyCode.S, KeyCode.O, KeyCode.S, KeyCode.I }));
	}

	/// <summary>
	/// ur mom
	/// </summary>
	private void HandleHui()
	{
		if (!Input.GetKeyDown(KeyCode.H) || copeRoutine != null)
		{
			return;
		}
		if (copeRoutine != null)
		{
			StopCoroutine(copeRoutine);
		}
		copeRoutine = StartCoroutine(Hui(0.5f, 0, new List<KeyCode> { KeyCode.H, KeyCode.U, KeyCode.I }));
	}

	private IEnumerator Hui(float timeLeft, int now, List<KeyCode> killYourself)
	{
		while (timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
			if (Input.GetKeyDown(killYourself[now]))
			{
				if (now == killYourself.Count - 1)
				{
					GameManager.Instance.allRooms.Where(x => x.GetComponent<RoomScript>()).ToList().ForEach(x => x.GetComponent<RoomScript>().ChangeDurability(1) );
					Debug.Log("HUI");
					Debug.Log($"Breakage!: {now}");
					break;
				}
				yield return Sosi(timeLeft, now + 1, killYourself);
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

	private IEnumerator Sosi(float timeLeft, int now, List<KeyCode> killYourself)
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
					Debug.Log("SOSI");
					Debug.Log($"Breakage!: {now}");
					break;
				}
				yield return Sosi(timeLeft, now + 1, killYourself);
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

	private IEnumerator Goida(float timeLeft, int now, List<KeyCode> killYourself)
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
				yield return Goida(timeLeft, now + 1, killYourself);
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
