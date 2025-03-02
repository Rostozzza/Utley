using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchRoomExercise : MonoBehaviour
{
	[Header("Problem Solver")]
	public TMP_InputField ribsAnswer;
	public TMP_InputField verticiesAnswer;
	[SerializeField] public bool answerTrigger;
	[SerializeField] private Transform pointsParent;
	[HideInInspector] public bool isTaskActive = false;
	private int ribs;
	private int verticies;
	[Header("Task Generator")]
	[SerializeField][Range(3, 50)] private int difficulty;
	[SerializeField] private GameObject weightPrefab;
	[SerializeField] private List<OgePointLogic> points;
	public LineRenderer lineRenderer;

	public void StartExercise(RoomScript room)
	{
		StartCoroutine(AnswerWaiter(room));
	}

	/// <summary>
	/// Generates task, answer and connects all OgePointLogics.
	/// </summary>
	/// <param name="steps"></param>
	/// <returns></returns>
	private int[] GenerateTask(int steps = 3)
	{
		verticies = Random.Range(steps, steps + 6);
		ribs = 0;
		points = pointsParent.GetComponentsInChildren<OgePointLogic>().ToList();
		pointsParent = points[0].transform.parent;
		int randomPointIndex = Random.Range(0, Mathf.Clamp(verticies, 0, points.Count));
		
		var currentPoint = points[randomPointIndex];
		Color red = new Color(1, 0, 0, 0.5f);
		currentPoint.SetColor(red);
		try
		{
			for (int i = 0; i < verticies; i++)
			{
				currentPoint.ConnectPoints();
				var connectedPoints = currentPoint.GetConnectedPoints();
				var previousPoint = currentPoint;
				randomPointIndex = Random.Range(0, connectedPoints.Count);
				currentPoint = connectedPoints[randomPointIndex];
				if (steps - i == 1)
				{
					currentPoint.SetColor(red);
					previousPoint.GetComponentsInChildren<LineRenderer>()[randomPointIndex].SetColors(red, red);
				}
				else
				{
					currentPoint.SetColor(red);
					previousPoint.GetComponentsInChildren<LineRenderer>()[randomPointIndex].SetColors(red, red);
				}
				Debug.Log($"Random point index: {randomPointIndex}");
				ribs++;
				//Vector3 averagePosition = previousPoint.transform.position + (currentPoint.transform.position - previousPoint.transform.position) / 2;
				//var weight = Instantiate(weightPrefab, pointsParent);
				//weight.transform.position = averagePosition;
				//int weightValue = Random.Range(1, difficulty);
				//weight.GetComponentInChildren<TextMeshProUGUI>(true).text = weightValue.ToString();
				//correctAnswer += weightValue;
			}
		}
		catch
		{
		}
		verticies = ribs + 1;
		return new int[] {verticies,ribs };
	}

	/// <summary>
	/// Waits for player's answer.
	/// </summary>
	/// <param name="roomToTarget"></param>
	/// <returns></returns>
	private IEnumerator AnswerWaiter(RoomScript roomToTarget)
	{
		pointsParent = roomToTarget.transform;
		GenerateTask();
		Camera.main.GetComponent<CameraController>().SetCameraLock(true);
		while (!answerTrigger)
		{
			yield return null;
		}
		answerTrigger = false;
		Time.timeScale = 1;
		Camera.main.GetComponent<CameraController>().SetCameraLock(false);
		if (int.Parse(verticiesAnswer.text) == verticies && int.Parse(ribsAnswer.text) == ribs)
		{
			Debug.Log("¬≈–Õ€… Œ“¬≈“");
			roomToTarget.SetWorkEfficiency(1);
		}
		else
		{
			Debug.Log("Œ“¬≈“ Õ≈¬≈–Õ€…");
			roomToTarget.SetWorkEfficiency(0.8f);
		}
		GameManager.Instance.SetIsGraphUsing(false);
		Camera.main.GetComponent<CameraController>().GoToTaskPoint(Vector3.zero, Vector3.zero);
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
