using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using Unity.Mathematics;

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
	[SerializeField] private List<GameObject> taskPrefabs;
	[SerializeField] private int playerDifficulty = 1; // sorry for using this instead of (difficulty). Idk how your var is working
	private RoomScript targetedRoom;

	[Serializable]
	public struct TaskPreset
	{
		public List<int> answers;
		public List<TMP_InputField> fields;
		public GameObject task;
		public int difficultLevel; // from 1 to ...3?
	}

	public void GenerateTask(RoomScript room)
	{
		GameManager.Instance.SetIsGraphUsing(true);
		MenuManager.Instance.problemSolverScreen.SetActive(true);
		targetedRoom = room;
		GenerateFromPreset();
	}

	public void GenerateFromPreset()
	{
		Camera.main.GetComponent<CameraController>().SetCameraLock(true);
		var preset = tasksPresets[tasksPresets.FindIndex(x => x.difficultLevel == TaskDifficultByPlayerDifficult(playerDifficulty))]; //[Random.Range(gridRange, Mathf.Clamp(gridRange + 2, 0, tasksPresets.Count))]; // also now task finds by player difficuty. I think it's makes more sense if we using "templates"
		task = preset.task;
		rightAnswers = preset.answers;
		allInputFields = preset.fields;
		StartCoroutine(ConnectAllPoints());
		task.SetActive(true);
	}

/// <summary>
/// Sometimes player difficulty may be higher than most difficult our task, so this method solving this problem. P.S. This approach assumes that tasks are arranged in increasing complexity without gaps.
/// </summary>
/// <param name="playerDifficulty"></param>
/// <returns></returns>
	private int TaskDifficultByPlayerDifficult(int playerDifficulty)
	{
		int highestDifficult = 0;
		tasksPresets.ForEach(x => highestDifficult = math.max(highestDifficult, x.difficultLevel));
		return (highestDifficult < playerDifficulty) ? highestDifficult : playerDifficulty;
	}

	private void ChangePlayerDifficulty(bool isPositive)
	{
		if (isPositive)
		{
			playerDifficulty++;
		}
		else
		{
			playerDifficulty--;
		}
		playerDifficulty = Math.Clamp(playerDifficulty, 1, 999);
	}

	private IEnumerator ConnectAllPoints()
	{
		yield return new WaitForSeconds(1.5f);
		foreach (var point in task.GetComponentsInChildren<OgePointLogic>(true))
		{
			point.ConnectPoints();
		}
		yield return task;
	}

	private void GenerateGirdLayout()
	{
		rightAnswers = new List<int>();
		gridLayout = new string[gridRange + 1, gridRange + 1];
		for (int i = 0; i <= gridRange; i++)
		{
			gridLayout[0, i] = $"�{i + 1}";
			gridLayout[i, 0] = $"�{i + 1}";
		}
		for (int i = 1; i <= gridRange; i++)
		{
			int randomAnswer = Random.Range(2, difficulty + 1);
			rightAnswers.Add(randomAnswer);
		}
	}

	public void SubmitAnswer()
	{
		GameManager.Instance.SetIsGraphUsing(true);
		for (int i = 0; i < rightAnswers.Count; i++)
		{
			try
			{
				if (rightAnswers[i] != int.Parse(allInputFields[i].text))
				{
					Debug.Log("НЕВЕРНО");
					rightAnswers = null;
					allInputFields.ForEach(x => x.text = "");
					allInputFields = null;
					task.SetActive(false);
					CreateNewExercise(task);
					Destroy(task, 0.1f);
					tasksPresets.Remove(tasksPresets.First(x => x.task == task));
					task = null;
					MenuManager.Instance.connectFurnacesScreen.SetActive(false);
					MenuManager.Instance.problemSolverScreen.SetActive(false);
					MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
					targetedRoom.SetWorkEfficiency(0.2f);
					Camera.main.GetComponent<CameraController>().SetCameraLock(false);
					MenuManager.Instance.problemSolverScreen.SetActive(false);
					targetedRoom.SetConeierScreen(false);
					return;
				}
			}
			catch
			{
				EventManager.callWarning.Invoke("Не все поля ответов заполнены!");
				return;
			}
		}
		rightAnswers = null;
		allInputFields = null;
		task.SetActive(false);
		CreateNewExercise(task);
		Destroy(task, 0.1f);
		tasksPresets.Remove(tasksPresets.First(x => x.task == task));
		task = null;
		gridRange++;
		MenuManager.Instance.connectFurnacesScreen.SetActive(false);
		MenuManager.Instance.problemSolverScreen.SetActive(false);
		MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
		targetedRoom.SetWorkEfficiency(1f);
		GameManager.Instance.TryProcessingRawAsterium();
		Debug.Log("ВЕРНО");
		ChangePlayerDifficulty(true);
		targetedRoom.SetConeierScreen(false);
		Camera.main.GetComponent<CameraController>().SetCameraLock(false);
		MenuManager.Instance.problemSolverScreen.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.E)) // Interrupts problem solving without giving answer (problem stays unsolved)
		{
			Time.timeScale = 1;
			GameManager.Instance.SetIsGraphUsing(false);
			rightAnswers = null;
			allInputFields = null;
			task.SetActive(false);
			CreateNewExercise(task);
			Destroy(task, 0.1f);
			tasksPresets.Remove(tasksPresets.First(x => x.task == task));
			task = null;
			MenuManager.Instance.connectFurnacesScreen.SetActive(false);
			MenuManager.Instance.problemSolverScreen.SetActive(false);
			MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
			Camera.main.GetComponent<CameraController>().SetCameraLock(false);
		}
	}

	private void CreateNewExercise(GameObject deletedTask) // asking for deleted task because we looking for same task from prefab
	{
		//var prefab = taskPrefabs[Random.Range(0, taskPrefabs.Count)];
		var prefab = taskPrefabs[taskPrefabs.FindIndex(x => x.name == deletedTask.name)];
		var newTask = Instantiate(prefab, this.gameObject.transform);
		newTask.name = newTask.name[..^7];
		tasksPresets.Add(TaskPrefabToPreset(newTask));
		newTask.SetActive(false);
	}

	private TaskPreset TaskPrefabToPreset(GameObject pref)
	{
		TaskPreset toReturn = new TaskPreset();
		if (pref.name == taskPrefabs[0].name)
		{
			toReturn.answers = new List<int> { 5, 1 };
			toReturn.difficultLevel = 1;
		}
		else if (pref.name == taskPrefabs[1].name)
		{
			toReturn.answers = new List<int> { 10, 7 };
			toReturn.difficultLevel = 2;
		}
		else if (pref.name == taskPrefabs[2].name)
		{
			toReturn.answers = new List<int> { 2, 5, 6, 8 };
			toReturn.difficultLevel = 3;
		}
		else
		{
			Debug.Log("<color=red>Что-то не так</color>");
		}
		toReturn.task = pref;
		toReturn.fields = pref.transform.Find("Inputs").GetComponentsInChildren<TMP_InputField>().ToList();
		return toReturn;
	}
}
