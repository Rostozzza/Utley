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
	private RoomScript targetedRoom;

	[Serializable]
	private struct SupplyTaskPreset
	{
		[SerializeField] public GameObject graphView;
		[SerializeField] public float answer;
	}

	public void InitializeTask(RoomScript room)
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

	public void SubmitAnswer()
	{
		var answerField = currentTask.graphView.GetComponentsInChildren<TMP_InputField>().First(x => x.gameObject.tag == "answerField");
		if (int.Parse(answerField.text) == currentTask.answer)
		{
			(targetedRoom as SupplyRoom).GetRoomsToEnpower();
			MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
			targetedRoom.SetWorkEfficiency(1f);
			Camera.main.GetComponent<CameraController>().SetCameraLock(false);
			MenuManager.Instance.problemSolverScreen.SetActive(false);
			MenuManager.Instance.graphExercise.gameObject.SetActive(false);
			return;
		}
		MenuManager.Instance.tabletAnimator.SetTrigger("CloseShop");
		targetedRoom.SetWorkEfficiency(0f);
		Camera.main.GetComponent<CameraController>().SetCameraLock(false);
		MenuManager.Instance.problemSolverScreen.SetActive(false);
		MenuManager.Instance.graphExercise.gameObject.SetActive(false);
		return;
	}
}
