using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class NumbersByTableExercise : MonoBehaviour
{
	[Header("Grid Settings")]
	[SerializeField] private GridLayoutGroup grid;
	[SerializeField] private GameObject gridCellPrefab;
	[SerializeField][Range(0, 6)] private int gridRange;
	[SerializeField][Range(10, 100)] private float gridScale;
	[Header("Input Settings")]
	[SerializeField] private GameObject inputPrefab;
	[SerializeField] private List<TMP_InputField> allInputFields;
	[Header("Task Generation")]
	[SerializeField][Range(2, 50)] private int difficulty;
	[SerializeField] private GameObject task;
	[SerializeField] private string[,] gridLayout;
	[SerializeField] private List<int> rightAnswers;
	[SerializeField] private List<TaskPreset> tasksPresets;
	private RoomScript targetedRoom;

	[Serializable]
	public struct TaskPreset
	{
		public List<int> answers;
		public List<TMP_InputField> fields;
		public GameObject task;
	}

	public void GenerateTask(RoomScript room)
	{
		targetedRoom = room;
		GenerateFromPreset();
	}

	public void GenerateFromPreset()
	{
		var preset = tasksPresets[Random.Range(gridRange, Mathf.Clamp(gridRange+2,0,tasksPresets.Count))];
		task = preset.task;
		rightAnswers = preset.answers;
		allInputFields = preset.fields;
		task.SetActive(true);
	}

	private void GenerateGirdLayout()
	{
		rightAnswers = new List<int>();
		gridLayout = new string[gridRange+1, gridRange+1];
		for (int i = 0; i <= gridRange; i++)
		{
			gridLayout[0, i] = $"П{i + 1}";
			gridLayout[i, 0] = $"П{i + 1}";
		}
		for (int i = 1; i <= gridRange; i++)
		{
			int randomAnswer = Random.Range(2, difficulty + 1);
			rightAnswers.Add(randomAnswer);
		}
	}

	public void SubmitAnswer()
	{
		for (int i = 0; i < rightAnswers.Count; i++)
		{
			if (rightAnswers[i] != int.Parse(allInputFields[i].text))
			{
				Debug.Log("Неверно");
				rightAnswers = null;
				allInputFields = null;
				task.SetActive(false);
				Destroy(task,0.1f);
				tasksPresets.Remove(tasksPresets.First(x => x.task == task));
				task = null;
				MenuManager.Instance.problemSolverScreen.SetActive(false);
				MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
				targetedRoom.SetWorkEfficiency(0.2f);
				return;
			}
		}
		rightAnswers = null;
		allInputFields = null;
		task.SetActive(false);
		Destroy(task, 0.1f);
		tasksPresets.Remove(tasksPresets.First(x => x.task == task));
		task = null;
		gridRange++;
		MenuManager.Instance.problemSolverScreen.SetActive(false);
		MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
		targetedRoom.SetWorkEfficiency(1f);
		GameManager.Instance.TryProcessingRawAsterium();
		Debug.Log("Верно");
	}
}
