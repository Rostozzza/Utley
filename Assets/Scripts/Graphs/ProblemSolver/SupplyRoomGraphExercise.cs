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
	private bool isListenerAdded = false;

	[Serializable]
	private struct SupplyTaskPreset
	{
		[SerializeField] public GameObject graphView;
		[SerializeField] public float answer;
	}

	private void Start()
	{
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

	public void SubmitAnswer(TMP_InputField field)
	{
		var answerField = field;
		
		Camera.main.GetComponent<CameraController>().GoToTaskPoint(Vector3.zero,Vector3.zero);
		if (int.Parse(answerField.text) == currentTask.answer)
		{
			(targetedRoom as SupplyRoom).GetRoomsToEnpower();
			Debug.Log("CORRECT");
			targetedRoom.SetWorkEfficiency(1f);
			Camera.main.GetComponent<CameraController>().SetCameraLock(false);
			return;
		}
		Debug.Log("INCORRECT");
		targetedRoom.SetWorkEfficiency(0f);
		Camera.main.GetComponent<CameraController>().SetCameraLock(false);
		return;
	}
}