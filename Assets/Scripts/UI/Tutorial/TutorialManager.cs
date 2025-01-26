
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class TutorialManager : MonoBehaviour
{
	[Header("Tutorial Settings")]
	[SerializeField] private GameObject kruzhochek;
	[SerializeField] private Transform tutorialCanvas;
	[SerializeField] private List<TutorialPart> sequence;
	private KeyCode keyCodeToCheck = KeyCode.None;
	private RoomScript roomToCheck = null;
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
		public Vector3 position;
		public Pointer pointer;
		public List<Condition> conditionsSequence;
		public Button buttonToCheck;
		public KeyCode keyToCheck;
		public string tagToCheck;
		public RoomScript roomToCheck;
		public RoomOutliner roomHighlight;
		public Vector3 krugPos;
		public float krugScale;
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
		OnClickLMB,
		OnRoomInfoCheck,
		OnRoomSelect,
		OnBearWorkStarted,
		OnEnergohoneySettingsOpened,
		OnEnergohoneySerringsSolved,
		OnUrsovaksSold
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
			if (part.roomToCheck != null)
			{
				roomToCheck = part.roomToCheck;
			}
			if (part.keyToCheck != KeyCode.None)
			{
				keyCodeToCheck = part.keyToCheck;
			}
			if (part.krugScale != 0)
			{
				kruzhochek.GetComponent<Image>().enabled = true;
				kruzhochek.transform.localPosition = part.krugPos;
				kruzhochek.transform.localScale *= part.krugScale;
			}
			else
			{
				kruzhochek.GetComponent<Image>().enabled = false;
			}
			tutorialView.localPosition = part.position;
			textOutput.text = part.text;
			yield return ConditionWaiter(part.conditionsSequence, part.buttonToCheck, part.tagToCheck, part.roomToCheck);
			if (part.roomHighlight != null)
			{
				part.roomHighlight.SetOutline(false);
			}
			roomToCheck = null;
			keyCodeToCheck = KeyCode.None;
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
					button.onClick.AddListener(StopWaiting);
					yield return WaitForEvent();
					button.onClick.RemoveListener(StopWaiting);
					Debug.Log("Sosal?");
					isButtonPressed = false;
					break;
				case Condition.OnClickLMB:
					while (!Input.GetMouseButtonDown(0)) yield return null;
					break;
				case Condition.OnRoomInfoCheck:
					while (!GameManager.Instance.allRooms.Where(x => x.GetComponent<RoomScript>()).Any(x => 
						{
							if (x.transform.Find("RoomInfo").gameObject.activeInHierarchy)
							{
								Debug.Log(x.transform);
								return true;
							}
							return false;
						})) yield return null;
					break;
				case Condition.OnBearSelect:
					EventManager.onBearSelected.AddListener(StopWaiting);
					yield return WaitForEvent();
					EventManager.onBearSelected.RemoveListener(StopWaiting);
					break;
				case Condition.OnBearMove:
					EventManager.onBearReachedDestination.AddListener(StopWaitingForRoomCheck);
					yield return WaitForEvent();
					EventManager.onBearReachedDestination.RemoveListener(StopWaitingForRoomCheck);
					break;
				case Condition.OnRoomSelect:
					EventManager.onRoomSelected.AddListener(StopWaiting);
					yield return WaitForEvent();
					EventManager.onRoomSelected.RemoveListener(StopWaiting);
					break;
				case Condition.OnBearWorkStarted:
					EventManager.onBearWorkStarted.AddListener(StopWaitingForRoomCheck);
					yield return WaitForEvent();
					EventManager.onBearWorkStarted.RemoveListener(StopWaitingForRoomCheck);
					break;
				case Condition.OnEnergohoneySettingsOpened:
					EventManager.onEnergohoneySettingsOpened.AddListener(StopWaiting);
					yield return WaitForEvent();
					EventManager.onEnergohoneySettingsOpened.RemoveListener(StopWaiting);
					break;
				case Condition.OnEnergohoneySerringsSolved:
					EventManager.onEnergohoneySettingsSolved.AddListener(StopWaiting);
					yield return WaitForEvent();
					EventManager.onEnergohoneySettingsSolved.RemoveListener(StopWaiting);
					break;
				case Condition.OnUrsovaksSold:
					EventManager.onUrsovaxSent.AddListener(StopWaiting);
					yield return WaitForEvent();
					EventManager.onUrsovaxSent.RemoveListener(StopWaiting);
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
		if (!GameManager.Instance.GetComponent<Canvas>().transform.Find(data.target.name))
		{
			Vector2 newPos;
			
			RectTransformUtility.ScreenPointToLocalPointInRectangle(tutorialCanvas.GetComponent<RectTransform>(), (Vector2)Camera.main.WorldToScreenPoint(data.target.position), Camera.main, out newPos);
			pointer.position = newPos;
			Debug.Log($"POINTER AT {newPos}");
			TryClearPointer();
			pointer.eulerAngles = new Vector3(0, 0, data.angleZ);
			pointer.localScale = new Vector3(data.length, data.length, data.length);
			pointerSlot = pointer.gameObject;
			while (pointer != null)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(tutorialCanvas.GetComponent<RectTransform>(), (Vector2)Camera.main.WorldToScreenPoint(data.target.position), Camera.main, out newPos);
				pointer.position = newPos;
			}
		}
		else
		{
			pointer.position = data.target.position;
		}
		pointer.eulerAngles = new Vector3(0, 0, data.angleZ);
		pointer.localScale = new Vector3(data.length, data.length, data.length);
		TryClearPointer();
		pointerSlot = pointer.gameObject;
	}

	/// <summary>
	/// Removes pointer from scene;
	/// </summary>
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
	/// Checks if bear that had triggered the event reached energohoney room and stops waiting;
	/// </summary>
	/// <param name="room"></param>
	private void StopWaitingForRoomCheck(RoomScript room)
	{
		if (room.GetType() == roomToCheck.GetType())
		{
			Debug.Log("reached room!");
			isButtonPressed = true;
			StopCoroutine(WaitForEvent());
		}
	}

	/// <summary>
	/// Stop event-waiting coroutine
	/// </summary>
	private void StopWaiting()
	{
		Debug.Log("clicked!");
		isButtonPressed = true;
		StopCoroutine(WaitForEvent());
	}

	/// <summary>
	/// Start waiting untill coroutine is disabled.
	/// </summary>
	private IEnumerator WaitForEvent()
	{
		while (!isButtonPressed && (keyCodeToCheck != KeyCode.None ? !Input.GetKeyDown(keyCodeToCheck) : true))
		{
			yield return null;
		}
		isButtonPressed = false;
	}
}
