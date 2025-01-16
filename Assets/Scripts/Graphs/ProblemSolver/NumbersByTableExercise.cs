using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class NumbersByTableExercise : MonoBehaviour
{
	[Header("Grid Settings")]
	[SerializeField] private GridLayoutGroup grid;
	[SerializeField] private GameObject gridCellPrefab;
	[SerializeField][Range(3, 8)] private int gridRange;
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

	public struct TaskPreset
	{
		public List<int> answers;
		public List<TMP_InputField> fields;
		public GameObject task;
	}

	public void GenerateTask()
	{
		GenerateFromPreset();
	}

	public void GenerateFromPreset()
	{
		var preset = tasksPresets[Random.Range(0, tasksPresets.Count)];
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
				task = null;
				rightAnswers = null;
				allInputFields = null;
				task.SetActive(false);
				Destroy(task,0.1f);
				tasksPresets.Remove(tasksPresets.First(x => x.task == task));
				task = null;
				return;
			}
		}
		task = null;
		rightAnswers = null;
		allInputFields = null;
		task.SetActive(false);
		Destroy(task, 0.1f);
		tasksPresets.Remove(tasksPresets.First(x => x.task == task));
		task = null;
		Debug.Log("Верно");
	}
}
