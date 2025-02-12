
using System;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class NumberSummationExercise : MonoBehaviour
{
	[Header("Problem Solver")]
	[SerializeField] public int answer;
	[SerializeField] private int rightAnswer;
	[SerializeField] public bool answerTrigger;
	[SerializeField] private Transform pointsParent;
	[HideInInspector] public bool isTaskActive = false;
	[Header("Task Generator")]
	[SerializeField][Range(1, 50)] private int difficulty;
	[SerializeField] private GameObject weightPrefab;
	[SerializeField] private List<OgePointLogic> points;
	public LineRenderer lineRenderer;

	/// <summary>
	/// Generates task, answer and connects all OgePointLogics.
	/// </summary>
	/// <param name="steps"></param>
	/// <returns></returns>
	private int GenerateTask(int steps = 3)
	{
		EventManager.onEnergohoneySettingsOpened.Invoke();
		points = pointsParent.GetComponentsInChildren<OgePointLogic>().ToList();
		int randomPointIndex = Random.Range(0, Mathf.Clamp(steps, 0, points.Count));
		foreach (var point in points)
		{
			point.ConnectPoints();
		}
		var currentPoint = points[randomPointIndex];
		currentPoint.SetColor(Color.red);
		int correctAnswer = 0;
		for (int i = 0; i < steps; i++)
		{
			var connectedPoints = currentPoint.GetConnectedPoints();
			var previousPoint = currentPoint;
			randomPointIndex = Random.Range(0, connectedPoints.Count);
			currentPoint = connectedPoints[randomPointIndex];
			if (steps - i == 1)
			{
				currentPoint.SetColor(Color.red);
			}
			else
			{
				currentPoint.SetColor(Color.red);
			}
			Vector3 averagePosition = previousPoint.transform.position + (currentPoint.transform.position - previousPoint.transform.position)/2;
			var weight = Instantiate(weightPrefab,pointsParent);
			weight.transform.position = averagePosition;
			int weightValue = Random.Range(1,difficulty);
			weight.GetComponent<TextMeshProUGUI>().text = weightValue.ToString();
			correctAnswer += weightValue;
		}
		
		Debug.Log($"Finished generating task! Answer: {correctAnswer}");
		return correctAnswer;
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
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) // Interrupts problem solving without giving answer (problem stays unsolved)
			{
				Time.timeScale = 1;
				Camera.main.GetComponent<CameraController>().SetCameraLock(false);
				roomToTarget.GivePermissionToContinue();
				GameManager.Instance.SetIsGraphUsing(false);
				ClearGraph();
				isTaskActive = false;
				MenuManager.Instance.problemSolverScreen.SetActive(false);
				MenuManager.Instance.graphExercise.gameObject.SetActive(false);
				yield break;
			}
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
			roomToTarget.SetWorkEfficiency(0.8f);
		}
		EventManager.onEnergohoneySettingsSolved.Invoke();
		ClearGraph();
		isTaskActive = false;
		MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
		MenuManager.Instance.problemSolverScreen.SetActive(false);
		MenuManager.Instance.graphExercise.gameObject.SetActive(false);
		GameManager.Instance.SetIsGraphUsing(false);
	}

	/// <summary>
	/// Clean up task after completion.
	/// </summary>
	public void ClearGraph()
	{
		pointsParent.GetComponentsInChildren<OgePointLogic>(true).ToList().ForEach(p => p.SetColor(Color.white));
		pointsParent.GetComponentsInChildren<LineRenderer>(true).ToList().ForEach(line => Destroy(line.gameObject,1f));
		pointsParent.GetComponentsInChildren<TextMeshProUGUI>(true).ToList().Where(text => !text.transform.parent.GetComponent<RectMask2D>() && !text.gameObject.CompareTag("dont_destroy_text")).ToList().ForEach(weight => Destroy(weight.gameObject, 1f));
	}

	/// <summary>
	/// Triggers answerWaiter.
	/// </summary>
	/// <param name="answerHolder"></param>
	public void GiveAnswer(GameObject answerHolder)
	{
		answer = Convert.ToInt32(answerHolder.GetComponent<TMP_InputField>().text);
		answerTrigger = true;
	}
}
