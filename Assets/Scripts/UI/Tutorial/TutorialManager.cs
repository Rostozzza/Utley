
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI textOutput;
	[SerializeField] private List<TutorialPart> sequence;

	[Serializable]
	private struct TutorialPart
	{
		public string text;
		public Vector2 position;
		public Pointer pointer;
		public List<Condition> conditionsSequence;
		public Button buttonToCheck;
		public string tagToCheck;
		public RoomScript roomToCheck;
		//Highlights
	}

	[Serializable]
	public struct Pointer
	{
		public Transform target;
		public Vector2 originDirection;
		public float length;
	}

	[Serializable]
	public enum Condition
	{
		OnCameraMove,
		OnButtonPress,
		OnMenuShown,
		OnBearSelect,
		OnBearMove,
	}

	private IEnumerator TutorialSequence()
	{
		foreach (var part in sequence)
		{
			yield return ConditionWaiter(part.conditionsSequence, part.buttonToCheck, part.tagToCheck, part.roomToCheck);
		}
	}

	private IEnumerator ConditionWaiter(List<Condition> conditions,Button button = null, string tag = null, RoomScript roomToWork = null)
	{
		foreach (var condition in conditions)
		{
			switch (condition)
			{
				case Condition.OnCameraMove:
					var cameraTransform = Camera.main.transform;
					var deltaCameraMovement = 0f;
					Vector2 previousPos = cameraTransform.position;
					while (deltaCameraMovement <= 5f)
					{
						deltaCameraMovement += ((Vector2)cameraTransform.position - previousPos).magnitude;
						previousPos = cameraTransform.position;
						yield return null;
					}
					break;
				case Condition.OnButtonPress:
					button.onClick.AddListener(OnButtonClick);
					yield return WaitForButtonClick();
					break;
			}
		}
	}

	private void OnButtonClick()
	{
		StopCoroutine(WaitForButtonClick());
	}

	private IEnumerator WaitForButtonClick()
	{
		while (true)
		{
			yield return null;
		}
	}
}
