using System;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class CosmodromeResistorsExercise : MonoBehaviour
{
	[Header("Problem Solver")]
	[SerializeField] public int answer;
	[SerializeField] private int rightAnswer;
	[SerializeField] public bool answerTrigger;
	[SerializeField] private Transform pointsParent;
	[HideInInspector] public bool isTaskActive = false;
	[Header("Task Generator")]
    [SerializeField] private List<GameObject> samples;
    [SerializeField] private GameObject sample;
	[SerializeField] private List<OgePointLogic> points;
	public LineRenderer lineRenderer;

	/// <summary>
	/// Generates task, answer and connects all OgePointLogics.
	/// </summary>
	/// <returns></returns>
	private int GenerateTask()
	{
        Debug.Log(samples.Count);
		sample = samples[Random.Range(1, samples.Count + 1) - 1];
        sample.SetActive(true);
        return sample.GetComponent<SampleStorage>().GetRightAnswer();
	}

	/// <summary>
	/// Waits for player's answer.
	/// </summary>
	/// <param name="roomToTarget"></param>
	/// <returns></returns>
	public IEnumerator AnswerWaiter(RoomScript roomToTarget)
	{
		rightAnswer = GenerateTask();
		Camera.main.GetComponent<CameraController>().SetCameraLock(true);
		while (!answerTrigger)
		{
			yield return null;
		}
		answerTrigger = false;
		Time.timeScale = 1;
		Camera.main.GetComponent<CameraController>().SetCameraLock(false);
		if (answer == rightAnswer)
		{
			Debug.Log("ВЕРНЫЙ ОТВЕТ");
			roomToTarget.SetWorkEfficiency(1);
		}
		else
		{
			Debug.Log("ОТВЕТ НЕВЕРНЫЙ");
			roomToTarget.SetWorkEfficiency(1 - 0.3f, false);
		}
        roomToTarget.GivePermissionToContinue();
		EventManager.onEnergohoneySettingsSolved.Invoke();
		GameManager.Instance.SetIsGraphUsing(false);
		ClearGraph();
		isTaskActive = false;
		MenuManager.Instance.problemSolverScreen.SetActive(false);
	}

	/// <summary>
	/// Clean up task after completion.
	/// </summary>
	public void ClearGraph()
	{
		//pointsParent.GetComponentsInChildren<OgePointLogic>(true).ToList().ForEach(p => p.SetColor(Color.white));
		//pointsParent.GetComponentsInChildren<LineRenderer>(true).ToList().ForEach(line => Destroy(line.gameObject,1f));
		//pointsParent.GetComponentsInChildren<TextMeshProUGUI>(true).ToList().Where(text => !text.transform.parent.GetComponent<RectMask2D>() && !text.gameObject.CompareTag("dont_destroy_text")).ToList().ForEach(weight => Destroy(weight.gameObject, 1f));
	}

    public void HideSample()
    {
        sample.SetActive(false);
    }

	/// <summary>
	/// Triggers answerWaiter.
	/// </summary>
	/// <param name="answerHolder"></param>
	public void GiveAnswer(GameObject answerHolder)
	{
		answer = Convert.ToInt32(answerHolder.GetComponent<TMP_InputField>().text);
		answerTrigger = true;
        answerHolder.GetComponent<TMP_InputField>().text = "";
	}
}
