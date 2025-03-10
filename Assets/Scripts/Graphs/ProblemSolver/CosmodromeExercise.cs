using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CosmodromeExercise : MonoBehaviour
{
	[Header("Problem Solver")]
	public TMP_InputField answerField;
	[SerializeField] public bool answerTrigger;
	private int correctAnswer = 0;
	[SerializeField] private Transform pointsParent;
	[HideInInspector] public bool isTaskActive = false;
	private int ribs;
	private int verticies;
	[Header("Task Generator")]
	[SerializeField][Range(3, 50)] private int difficulty;
	[SerializeField][Range(0f, 1f)] private float radiusRandomOffset;
	[SerializeField] private GameObject weightPrefab;
	[SerializeField] private List<OgePointLogic> shufflePoints;
	[SerializeField] private OgePointLogic startPoint;
	[SerializeField] private OgePointLogic endPoint;
	[SerializeField] private Transform generationCenterPoint;
	[SerializeField] private float generationRadius;
	[SerializeField] private Transform startAxisTransform;
	[SerializeField] private Transform endAxisTransform;



	private void ResetShuffledPoints()
	{
		shufflePoints = pointsParent.GetComponentsInChildren<OgePointLogic>(true).ToList();
		shufflePoints.Remove(startPoint);
		shufflePoints.Remove(endPoint);
	}

	/// <summary>
	/// Generates task, answer and connects all OgePointLogics.
	/// </summary>
	/// <param name="steps"></param>
	/// <returns></returns>
	private int GenerateTask()
	{
		Vector2 startAxis, endAxis, dir;

		//float radAngStart = startAxisTransform.rotation.z * Mathf.Deg2Rad;
		//startAxis = new Vector2(Mathf.Cos(radAngStart), Mathf.Sin(radAngStart));

		//float radAngEnd = endAxisTransform.rotation.z * Mathf.Deg2Rad;
		//endAxis = new Vector2(Mathf.Cos(radAngEnd), Mathf.Sin(radAngEnd));

		startAxis = startAxisTransform.rotation * Vector2.right;
		endAxis = endAxisTransform.rotation * Vector2.right;

		correctAnswer = 0;

		ResetShuffledPoints();

		var previousPoint = startPoint;
		int cycles = shufflePoints.Count;
		float radiansAngle = Mathf.Atan2(startAxis.y, startAxis.x);
		dir = new Vector2(Mathf.Cos(radiansAngle), Mathf.Sin(radiansAngle));

		for (int i = 0; i < cycles; i++)
		{
			int randNum = Random.Range(0, shufflePoints.Count);
			var currentPoint = shufflePoints[randNum];
			currentPoint.transform.position = generationCenterPoint.position + (Vector3)(dir * (generationRadius + Random.Range(-radiusRandomOffset, radiusRandomOffset)));
			shufflePoints.Remove(currentPoint);
			currentPoint.AddPointToConnected(previousPoint);
			Debug.Log("ShuffledPoint");
			Vector3 averagePosition = previousPoint.transform.position + (currentPoint.transform.position - previousPoint.transform.position) / 2;
			var weight = Instantiate(weightPrefab, pointsParent);
			weight.transform.position = averagePosition;
			int weightValue = Random.Range(1, difficulty);
			weight.GetComponentInChildren<TextMeshProUGUI>(true).text = weightValue.ToString();
			correctAnswer += weightValue;

			previousPoint = currentPoint;

			// Rotation calc;
			float angleIncrement = (Mathf.Atan2(startAxis.y, startAxis.x) - Mathf.Atan2(endAxis.y, endAxis.x)) / (cycles - 1);
			radiansAngle += angleIncrement;
			dir = new Vector2(Mathf.Cos(radiansAngle), Mathf.Sin(radiansAngle));
		}
		previousPoint.AddPointToConnected(endPoint);
		shufflePoints.Remove(previousPoint);

		ResetShuffledPoints();

		foreach (var point in shufflePoints)
		{
			point.ConnectPoints();
		}
		Debug.Log("GENERATED TASK AND CONNECTED POINTS!");
		return correctAnswer;
	}

	/// <summary>
	/// Waits for player's answer.
	/// </summary>
	/// <param name="roomToTarget"></param>
	/// <returns></returns>
	public IEnumerator AnswerWaiter(RoomScript roomToTarget)
	{
		Debug.Log("WAITING FOR ANSWER");
		GenerateTask();
		Camera.main.GetComponent<CameraController>().SetCameraLock(true);
		while (!answerTrigger)
		{
			yield return null;
		}
		answerTrigger = false;
		Time.timeScale = 1;
		Camera.main.GetComponent<CameraController>().SetCameraLock(false);
		if (int.Parse(answerField.text) == correctAnswer)
		{
			Debug.Log("������ �����");
			roomToTarget.SetWorkEfficiency(1);
		}
		else
		{
			Debug.Log("����� ��������");
			roomToTarget.SetWorkEfficiency(0.8f);
		}
		GameManager.Instance.SetIsGraphUsing(false);
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
		//pointsParent.GetComponentsInChildren<OgePointLogic>(true).ToList().ForEach(p => p.SetColor(Color.white));
		pointsParent.GetComponentsInChildren<LineRenderer>(true).ToList().ForEach(line => Destroy(line.gameObject, 1f));
		pointsParent.GetComponentsInChildren<TextMeshProUGUI>(true).ToList().Where(text => !text.transform.parent.GetComponent<RectMask2D>() && !text.gameObject.CompareTag("dont_destroy_text")).ToList().ForEach(weight => Destroy(weight.gameObject, 1f));
	}

	/// <summary>
	/// Triggers answerWaiter.
	/// </summary>
	/// <param name="answerHolder"></param>
	public void GiveAnswer()
	{
		answerTrigger = true;
	}
}
