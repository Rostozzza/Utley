
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
		pointsParent = points[0].transform.parent;
		int randomPointIndex = Random.Range(0, Mathf.Clamp(steps, 0, points.Count));
		foreach (var point in points)
		{
			point.ConnectPoints();
		}
		var currentPoint = points[randomPointIndex];
		Color red = new Color(1,0,0,0.5f);
		currentPoint.SetColor(red);
		int correctAnswer = 0;
		for (int i = 0; i < steps; i++)
		{
			var connectedPoints = currentPoint.GetConnectedPoints();
			var previousPoint = currentPoint;
			randomPointIndex = Random.Range(0, connectedPoints.Count);
			currentPoint = connectedPoints[randomPointIndex];
			if (steps - i == 1)
			{
				currentPoint.SetColor(red);

				previousPoint.GetComponentsInChildren<LineRenderer>()[randomPointIndex].SetColors(red,red);
			}
			else
			{
				currentPoint.SetColor(red);
				previousPoint.GetComponentsInChildren<LineRenderer>()[randomPointIndex].SetColors(red,red);
			}
			Vector3 averagePosition = previousPoint.transform.position + (currentPoint.transform.position - previousPoint.transform.position)/2;
			var weight = Instantiate(weightPrefab,pointsParent);
			weight.transform.position = averagePosition;
			int weightValue = Random.Range(1,difficulty);
			weight.GetComponentInChildren<TextMeshProUGUI>(true).text = weightValue.ToString();
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
		pointsParent = roomToTarget.transform;
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
			roomToTarget.SetWorkEfficiency(0.8f);
		}
		EventManager.onEnergohoneySettingsSolved.Invoke();
		GameManager.Instance.SetIsGraphUsing(false);
		Camera.main.GetComponent<CameraController>().GoToTaskPoint(Vector3.zero,Vector3.zero);
		ClearGraph();
		isTaskActive = false;
		MenuManager.Instance.problemSolverScreen.SetActive(false);
		gameObject.SetActive(false);
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
	public void GiveAnswer(int answer)
	{
		this.answer = answer;
		answerTrigger = true;
	}
}
