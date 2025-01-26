
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class TutorialManager : MonoBehaviour
{
	[Header("Tutorial Settings")]
	[SerializeField] private Transform tutorialCanvas;
	[SerializeField] private List<TutorialPart> sequence;
	[Header("View Settings")]
	[SerializeField] private TextMeshProUGUI textOutput;
	[SerializeField] private Transform tutorialView;
	[Header("Pointer Settings")]
	private GameObject pointerSlot;
	[SerializeField] private GameObject pointerPrefab;
	[Header("Hidden settings")]
	private bool isButtonPressed = false;

	/// <summary>
	/// Tutorial part, used in tutorial sequence. When some settings are unneeded, set them to Null.
	/// </summary>
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
		public RoomOutliner roomHighlight;
	}

	/// <summary>
	/// Data storage nessesary for creating arrows in tutorial. If no arrow wanted, leave field "target" empty =].
	/// </summary>
	[Serializable]
	public struct Pointer
	{
		public Transform target;
		public float angleZ;
		public float length;
	}

	/// <summary>
	/// Action, required for tutorial to move on to the next part
	/// </summary>
	[Serializable]
	public enum Condition
	{
		OnCameraMove,
		OnButtonPress,
		OnMenuShown,
		OnBearSelect,
		OnBearMove,
		OnClickLMB
	}

	private void Start()
	{
		tutorialView.gameObject.SetActive(true);
		StartCoroutine(TutorialSequence());
	}

	/// <summary>
	/// Tutorial sequence executioner. Turns on every needed parameter of TutorialPart and waits for required conditions.
	/// </summary>
	/// <returns></returns>
	private IEnumerator TutorialSequence()
	{
		foreach (var part in sequence)
		{
			if (part.pointer.target != null)
			{
				SpawnPointer(part.pointer);
			}
			if (part.roomHighlight != null)
			{
				part.roomHighlight.SetOutline(true);
			}
			tutorialView.localPosition = part.position;
			textOutput.text = part.text;
			yield return ConditionWaiter(part.conditionsSequence, part.buttonToCheck, part.tagToCheck, part.roomToCheck);
			if (part.roomHighlight != null)
			{
				part.roomHighlight.SetOutline(false);
			}
			TryClearPointer();
		}
	}

	/// <summary>
	/// Waits for specific conditions.
	/// </summary>
	/// <param name="conditions"></param>
	/// <param name="button"></param>
	/// <param name="tag"></param>
	/// <param name="roomToWork"></param>
	/// <returns></returns>
	private IEnumerator ConditionWaiter(List<Condition> conditions, Button button = null, string tag = null, RoomScript roomToWork = null)
	{
		foreach (var condition in conditions)
		{
			switch (condition)
			{
				case Condition.OnCameraMove:
					var cameraTransform = Camera.main.transform;
					var pathCameraMovement = 0f;
					Vector2 previousPos = cameraTransform.position;
					while (pathCameraMovement <= 0f) // Earlier was 50f, but Egor said that player should just move camera, not overcome the distance(🤡).
					{
						pathCameraMovement += MathF.Abs(((Vector2)cameraTransform.position - previousPos).magnitude);
						previousPos = cameraTransform.position;
						yield return null;
					}
					break;
				case Condition.OnButtonPress:
					button.onClick.AddListener(OnButtonClick);
					yield return WaitForButtonClick();
					Debug.Log("Sosal?");
					isButtonPressed = false;
					break;
				case Condition.OnClickLMB:
					while (!Input.GetMouseButtonDown(0)) yield return null;
					break;
			}
		}
	}

	/// <summary>
	/// Spawns an arrow on tutorial canvas accordiong to data provided in Pointer class.
	/// </summary>
	/// <param name="data"></param>
	private void SpawnPointer(Pointer data)
	{
		var pointer = Instantiate(pointerPrefab, tutorialCanvas).transform;
		pointer.position = data.target.position;
		pointer.eulerAngles = new Vector3(0, 0, data.angleZ);
		pointer.localScale = new Vector3(data.length, data.length, data.length);
		TryClearPointer();
		pointerSlot = pointer.gameObject;
	}

	private void TryClearPointer()
	{
		if (pointerSlot == null)
		{
			return;
		}
		Destroy(pointerSlot);
		pointerSlot = null;
	}

	/// <summary>
	/// Start waiting untill coroutine is disabled.
	/// </summary>
	private void OnButtonClick()
	{
		Debug.Log("clicked!");
		isButtonPressed = true;
		StopCoroutine(WaitForButtonClick());
	}

	private IEnumerator WaitForButtonClick()
	{
		while (!Input.GetMouseButtonDown(1) && !isButtonPressed)
		{
			yield return null;
		}
	}
}
