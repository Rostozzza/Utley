using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

public class UnitScript : MonoBehaviour
{
	[SerializeField] public bool chased;
	[SerializeField] public float speed = 5f;
	[SerializeField] public bool isBusy;
    [SerializeField] private Animator animator;
	private bool onLadder;
	private Rigidbody rb;
	private Vector3 dir;
	private int laddersAmount;
	private GameObject nearWorkStation;
	private Coroutine randomWalk;
	public bool selectable = true;
	public MeshRenderer furSlot;
	public MeshRenderer topSlot;
	public MeshRenderer bottomSlot;
	[Header("Bear Stats")]
	public Qualification job;
	public float level;
	public Mesh fur;
	public Mesh top;
	public Mesh bottom;
	public bool isBoosted = false;
	private Coroutine boostHolder = null;
	public Bear bearModel;
	public bool isBearBusy = false;
	private BearStatusController statusPanel;
	[Header("Bear UI")]
	[SerializeField] private GameObject statsScreen;
	[SerializeField] private TextMeshProUGUI nameField;
	[SerializeField] private TextMeshProUGUI qualificationField;
	[SerializeField] private TextMeshProUGUI levelField;
	[SerializeField] private Transform effectsGrid;
	[SerializeField] private SpriteRenderer marker;
	[SerializeField] private string Name;
	[SerializeField] private string strFromWork = "Не занят";
	[SerializeField] public Sprite avatar;
	[Header("VFX")]
	public GameObject expParticle;
	[SerializeField] private ParticleSystem freezeParticle;
	[SerializeField] private ParticleSystem breathParticle;
	[SerializeField] private ParticleSystem boostParticle;
	[SerializeField] private ParticleSystem researchParticle;

	private void Awake()
	{
		bearModel.Name = Name;
	}

	private void Start()
	{
		statusPanel = GameManager.Instance.bearStatusListController.CreateBearStatus(this);
		marker = transform.Find("Marker").GetComponent<SpriteRenderer>();
		SetMarker(false);
		//UpdateStatsScreen();
		statusPanel.UpdateLevel(level);
		//StartCoroutine(WalkCycle());
	}

	public void LoadDataFromModel(Bear model)
	{
		bearModel = model;
		//Load and set nessesary data from current model;
	}

	private void UpdateStatsScreen()
	{
		//nameField.text = bearModel.Name;
		//qualificationField.text = bearModel.Qualification.ToString();
		switch (level)
		{
			case 1:
				levelField.text = "I";
				break;
			case 2:
				levelField.text = "II";
				break;
			case 3:
				levelField.text = "III";
				break;
			case 4:
				levelField.text = "IV";
				break;
			case 5:
				levelField.text = "V";
				break;
		}
	}

	private void Update()
	{
		onLadder = laddersAmount > 0;
		if (GetComponent<UnitMovement>().currentRoutine == null && !isBearBusy && randomWalk == null && !GetComponentInChildren<Animator>().GetBool("Work"))
		{
			randomWalk = StartCoroutine(WalkCycle());
		}
		else if ((GetComponent<UnitMovement>().currentRoutine != null || isBearBusy || GetComponentInChildren<Animator>().GetBool("Work")) && randomWalk != null)
		{
			StopCoroutine(randomWalk);
			randomWalk = null;
			GetComponentInChildren<Animator>().speed = 1f;
		}
	}

	private IEnumerator WalkCycle()
	{
		yield return new WaitForSeconds(2 + Random.value * 3f);
		float startX = transform.position.x;
		while (true)
		{
			float randPosToWalkX = startX + 2f + ((Random.value - 0.5f) * 5);
			GetComponentInChildren<Animator>().speed = 0.4f;
			GetComponentInChildren<Animator>().SetBool("Walk", true);
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 90 * Mathf.Sign(randPosToWalkX - transform.position.x), 0);
			while (!(randPosToWalkX - 0.01f <= transform.position.x && transform.position.x <= randPosToWalkX + 0.01f))
			{
				transform.Translate(new Vector3(Mathf.Sign(randPosToWalkX - transform.position.x), 0, 0) * Time.deltaTime);
				yield return null;
			}
			GetComponentInChildren<Animator>().SetBool("Walk", false);
			yield return new WaitForSeconds(2 + Random.value * 5);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("ladder_room"))
		{
			laddersAmount++;
		}
		else if (other.CompareTag("work_station"))
		{
			nearWorkStation = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("ladder_room"))
		{
			laddersAmount--;
		}
	}

	public void LevelUpBear()
	{
		level = Mathf.Clamp(level + 1f/(int)(2 * Mathf.Pow(level, 1.2f)), 1, 5);
		statusPanel.UpdateLevel(level);
		expParticle.SetActive(false);
		statusPanel.UpdateLoveWork(false);
	}

	public void ChooseUnit()
	{
		chased = true;
	}

	/// <summary>
	/// Bear becomes busy and cannot be selected
	/// </summary>
	public void CannotBeSelected()
	{
		selectable = false;
	}

	/// <summary>
	/// Bear becomes free and can be selected
	/// </summary>
	public void CanBeSelected()
	{
		selectable = true;
	}

	public void SelectUnit()
	{
		GameManager.Instance.selectedUnit = gameObject;
		EventManager.onBearSelected.Invoke();
		GameManager.Instance.ShowAvailableAssignments();
		SetMarker(true);
	}

	public void StartMoveInRoom(RoomScript.Resources roomType, List<Vector3> points, GameObject obj)
	{
		StartCoroutine(MoveInRoom(roomType, points, obj));
	}

	public IEnumerator MoveInRoom(RoomScript.Resources roomType, List<Vector3> walkPoints, GameObject obj)
	{
		Coroutine walkingCoroutine;
		switch (roomType)
		{
			case RoomScript.Resources.Energohoney:
				walkingCoroutine = StartCoroutine(EnergohoneyWalkBehaviour(walkPoints, obj));
				while (obj.GetComponent<RoomScript>().status == RoomScript.Status.Busy)
				{
					yield return null;
				}
				StopCoroutine(walkingCoroutine);
				walkingCoroutine = null;
				break;
			case RoomScript.Resources.Bed:
				walkingCoroutine = StartCoroutine(BedroomWalkBehaviour(walkPoints, obj));
				EventManager.onBearWorkStarted.Invoke(GetComponent<UnitMovement>().currentRoom);
				while (obj.GetComponent<RoomScript>().status == RoomScript.Status.Busy)
				{
					yield return null;
				}
				StopCoroutine(walkingCoroutine);
				walkingCoroutine = null;
				break;
			case RoomScript.Resources.Build:
				walkingCoroutine = StartCoroutine(BuildBehaviour(obj));
				while (obj.GetComponent<BuilderRoom>().GetWait())
				{
					yield return null;
				}
				StopCoroutine(walkingCoroutine);
				GetComponentInChildren<Animator>().SetBool("Work", false);
				walkingCoroutine = null;
				break;
			case RoomScript.Resources.Research:
				walkingCoroutine = StartCoroutine(ResearchBehaviour(walkPoints, obj));
				while (obj.GetComponent<RoomScript>().status == RoomScript.Status.Busy)
				{
					yield return null;
				}
				StopCoroutine(walkingCoroutine);
				walkingCoroutine = null;
				break;
		}
	}

	private IEnumerator ResearchBehaviour(List<Vector3> walkPoints, GameObject obj)
	{
		Vector3 chosenPoint;
		chosenPoint = walkPoints[Random.Range(0, walkPoints.Count - 1)];
		GetComponentInChildren<Animator>().StopPlayback();
		GetComponentInChildren<Animator>().speed = 1;
		GetComponentInChildren<Animator>().SetBool("Walk", true);
		GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 90 * Mathf.Sign(chosenPoint.x - transform.position.x), 0);
		while (!(chosenPoint.x - 0.01f <= transform.position.x && transform.position.x <= chosenPoint.x + 0.01f))
		{
			transform.Translate(new Vector3(Mathf.Sign(chosenPoint.x - transform.position.x), 0, 0) * Time.deltaTime);
			yield return null;
		}
		GetComponentInChildren<Animator>().SetBool("Walk", false);
		GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 0, 0);
		GetComponentInChildren<Animator>().SetBool("Work", true);
		while (obj.GetComponent<RoomScript>().status == RoomScript.Status.Busy)
		{
			yield return null;
		}
		GetComponentInChildren<Animator>().SetBool("Work", false);
	}

	private IEnumerator BuildBehaviour(GameObject obj)
	{
		EventManager.onBearWorkStarted.Invoke(GetComponent<UnitMovement>().currentRoom);
		GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 0, 0);
		GetComponentInChildren<Animator>().SetBool("Work", true);
		while (obj.GetComponent<BuilderRoom>().GetWait())
		{
			yield return null;
		}
		GetComponentInChildren<Animator>().SetBool("Work", false);
	}

	private IEnumerator BedroomWalkBehaviour(List<Vector3> walkPoints, GameObject obj)
	{
		Vector3 chosenPoint;
		chosenPoint = walkPoints[Random.Range(0, walkPoints.Count - 1)];
		GetComponentInChildren<Animator>().StopPlayback();
		GetComponentInChildren<Animator>().speed = 1;
		GetComponentInChildren<Animator>().SetBool("Walk", true);
		GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 90 * Mathf.Sign(chosenPoint.x - transform.position.x), 0);
		while (!(chosenPoint.x - 0.01f <= transform.position.x && transform.position.x <= chosenPoint.x + 0.01f))
		{
			transform.Translate(new Vector3(Mathf.Sign(chosenPoint.x - transform.position.x), 0, 0) * Time.deltaTime);
			yield return null;
		}
		GetComponentInChildren<Animator>().SetBool("Walk", false);
	}

	private IEnumerator EnergohoneyWalkBehaviour(List<Vector3> walkPoints, GameObject obj)
	{
		Vector3 chosenPoint;
		GetComponentInChildren<Animator>().StopPlayback();
		GetComponentInChildren<Animator>().speed = 1f;
		while (obj.GetComponent<RoomScript>().status == RoomScript.Status.Busy)
		{
			GetComponentInChildren<Animator>().speed = 1;
			GetComponentInChildren<Animator>().SetBool("Walk", true);
			chosenPoint = walkPoints[Random.Range(0, walkPoints.Count - 1)];
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 90 * Mathf.Sign(chosenPoint.x - transform.position.x), 0);
			while (!(chosenPoint.x - 0.01f <= transform.position.x && transform.position.x <= chosenPoint.x + 0.01f))
			{
				transform.Translate(new Vector3(Mathf.Sign(chosenPoint.x - transform.position.x), 0, 0) * Time.deltaTime);
				yield return null;
			}
			GetComponentInChildren<Animator>().SetBool("Walk", false);
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 0, 0);
			GetComponentInChildren<Animator>().SetBool("Work", true);
			yield return new WaitForSeconds(5f);
			GetComponentInChildren<Animator>().SetBool("Work", false);
			GetComponentInChildren<Animator>().SetBool("Walk", true);
			chosenPoint = walkPoints[3];
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 90 * Mathf.Sign(chosenPoint.x - transform.position.x), 0);
			while (!(chosenPoint.x - 0.01f <= transform.position.x && transform.position.x <= chosenPoint.x + 0.01f))
			{
				transform.Translate(new Vector3(Mathf.Sign(chosenPoint.x - transform.position.x), 0, 0) * Time.deltaTime);
				yield return null;
			}
			GetComponentInChildren<Animator>().SetBool("Walk", false);
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 0, 0);
			GetComponentInChildren<Animator>().SetBool("Work", true);
			yield return new WaitForSeconds(3f);
			GetComponentInChildren<Animator>().SetBool("Work", false);
		}
		GetComponentInChildren<Animator>().speed = 3;
	}

	public void SetBusy(bool isBusy)
	{
		isBearBusy = isBusy;
		statusPanel.UpdateState(isBusy);
		GetComponentInChildren<Animator>().SetBool("Work", false);
		GetComponentInChildren<Animator>().SetBool("Walk", false);
		if (isBearBusy)
		{
			EventManager.onBearWorkStarted.Invoke(GetComponent<UnitMovement>().currentRoom);
		}
	}

	public void Boost()
	{
		if (boostHolder != null)
		{
			StopCoroutine(boostHolder);
		}
		boostParticle.Play();
		boostHolder = StartCoroutine(BoostTimer());
	} 

	public void StartBreath()
	{
		breathParticle.Play();
	}

	public void StopBreath()
	{
		breathParticle.Stop();
	}

	public void StartFreezing()
	{
		freezeParticle.Play();
	}

	public void StopFreezing()
	{
		freezeParticle.Play();
	}

	public void StartResearching()
	{
		researchParticle.Play();
	}

	public void StopResearching()
	{
		researchParticle.Stop();
	}

	private IEnumerator BoostTimer()
	{
		isBoosted = true;
		yield return new WaitForSeconds(150);
		isBoosted = false;
		boostParticle.Stop();
	}

	public void SetMarker(bool set)
	{
		marker.enabled = set;
		statusPanel.SetSelect(set);
	}

	public void SetStatsScreen(bool set)
	{
		if (statsScreen)
		{
			statsScreen.SetActive(set);
		}
	}
	public void SetStatsScreen()
	{
		if (statsScreen)
		{
			statsScreen.SetActive(!statsScreen.activeSelf);
		}
	}

	public BearStatusController GetStatusPanel()
	{
		return statusPanel;
	}

	public string GetWorkStr()
	{
		return strFromWork;
	}

	public void SetWorkStr(string set)
	{
		strFromWork = set;
		statusPanel.UpdateWorkStr(strFromWork);
	}

    public enum States
    {
        Idle,
        Walk,
        Working
    }
}
