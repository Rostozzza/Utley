using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Random = UnityEngine.Random;
using System;
using System.Collections;
using System.Linq;
using static NumbersByTableExercise;

public class SupplyRoomGraphExercise : MonoBehaviour
{
	[Header("Task Generation")]
	[SerializeField] private SupplyTaskPreset currentTask;
	[SerializeField] private List<SupplyTaskPreset> tasks;
	[SerializeField] private List<GameObject> taskPrefabs;
	private RoomScript targetedRoom;

	[Serializable]
	private struct SupplyTaskPreset
	{
		[SerializeField] public GameObject graphView;
		[SerializeField] public float answer;
	}

	public void InitializeTask(RoomScript room)
	{
		GameManager.Instance.SetIsGraphUsing(true);
		EventManager.onSupplySettingsOpened.Invoke();
		Camera.main.GetComponent<CameraController>().SetCameraLock(true);
		targetedRoom = room;
		currentTask = tasks[Random.Range(0, tasks.Count)];
		StartCoroutine(ConnectAllPoints());
	}

	private IEnumerator ConnectAllPoints()
	{
		yield return new WaitForSeconds(1.5f);
		foreach (var point in currentTask.graphView.GetComponentsInChildren<OgePointLogic>(true))
		{
			point.ConnectPoints();
		}

		currentTask.graphView.SetActive(true);
	}

	public void SubmitAnswer()
	{
		Debug.Log("Отправили ответ");
		var answerField = currentTask.graphView.GetComponentsInChildren<TMP_InputField>().First(x => x.gameObject.tag == "answerField");
		if (answerField.text == "")
		{
			EventManager.callWarning.Invoke("Поле ответа пустое!");
			return;
		}
		EventManager.onSupplyRoomSettingsSolved.Invoke();
		Destroy(currentTask.graphView);
		tasks.Remove(currentTask);
		var prefab = taskPrefabs[Random.Range(0, taskPrefabs.Count)];
		var newTask = Instantiate(prefab, this.gameObject.transform);
		newTask.name = newTask.name[..^7];
		tasks.Add(TaskPrefabToPreset(newTask));
		newTask.SetActive(false);
		if (int.Parse(answerField.text) == currentTask.answer)
		{
			(targetedRoom as SupplyRoom).GetRoomsToEnpower();
			MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
			targetedRoom.SetWorkEfficiency(1f);
			Camera.main.GetComponent<CameraController>().SetCameraLock(false);
			MenuManager.Instance.problemSolverScreen.SetActive(false);
			MenuManager.Instance.graphExercise.gameObject.SetActive(false);
			GameManager.Instance.SetIsGraphUsing(false);
			targetedRoom.SetConeierScreen(false);
			answerField.text = "";
			return;
		}
		MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
		targetedRoom.SetWorkEfficiency(0f);
		Camera.main.GetComponent<CameraController>().SetCameraLock(false);
		MenuManager.Instance.problemSolverScreen.SetActive(false);
		MenuManager.Instance.graphExercise.gameObject.SetActive(false);
		GameManager.Instance.SetIsGraphUsing(false);
		targetedRoom.SetConeierScreen(false);
		answerField.text = "";
		return;
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) // Interrupts problem solving without giving answer (problem stays unsolved)
		{
			Time.timeScale = 1;
			MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
			Camera.main.GetComponent<CameraController>().SetCameraLock(false);
			GameManager.Instance.SetIsGraphUsing(false);
			MenuManager.Instance.problemSolverScreen.SetActive(false);
			Camera.main.GetComponent<CameraController>().SetCameraLock(false);
			MenuManager.Instance.problemSolverScreen.SetActive(false);
			MenuManager.Instance.graphExercise.gameObject.SetActive(false);
			GameManager.Instance.SetIsGraphUsing(false);
			Destroy(currentTask.graphView);
			tasks.Remove(currentTask);
			var prefab = taskPrefabs[Random.Range(0, taskPrefabs.Count)];
			var newTask = Instantiate(prefab, this.gameObject.transform);
			newTask.name = newTask.name[..^7];
			tasks.Add(TaskPrefabToPreset(newTask));
			newTask.SetActive(false);
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			SubmitAnswer();
		}
    }

	private SupplyTaskPreset TaskPrefabToPreset(GameObject pref)
	{
		SupplyTaskPreset toReturn = new SupplyTaskPreset();
		if (pref.name == taskPrefabs[0].name)
		{
			toReturn.answer = 3;
			toReturn.graphView = pref;
		}
		else if (pref.name == taskPrefabs[1].name)
		{
			toReturn.answer = 10;
			toReturn.graphView = pref;
		}
		else
		{
			Debug.Log("Что-то не так");
		}
		return toReturn;
	}
}
