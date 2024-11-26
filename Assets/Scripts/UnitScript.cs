using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
	public Bear bearModel;
	[Header("Bear UI")]
	[SerializeField] private GameObject statsScreen;
	[SerializeField] private TextMeshProUGUI nameField;
	[SerializeField] private TextMeshProUGUI qualificationField;
	[SerializeField] private TextMeshProUGUI levelField;
	[SerializeField] private Transform effectsGrid;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		bearModel = new Bear { Name = "Барак Обама",Qualification = Qualification.beekeeper};//DONT FORGET TO MAKE JSON SAVE/LOAD SYSTEM!
		UpdateStatsScreen();
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
	}

	private IEnumerator WalkCycle()
	{
		dir.x = Random.Range(0, 2) == 1 ? 1 : -1;
		float timer = 7f + Random.value;
		while (timer > 0f)
		{
			//rb.linearVelocity = new Vector3(dir.x, onLadder ? 0f : rb.linearVelocity.y, 0f);
			// Uncomment if wanna see cool rays-detectors
			Debug.DrawRay(transform.position, Vector3.right, Color.yellow);
			Debug.DrawRay(transform.position, Vector3.left, Color.yellow);
			
			RaycastHit hit;
			if (dir.x == 1)
			{
				Ray rayR = new Ray(transform.position, Vector3.right);
				if (Physics.Raycast(rayR, out hit, 1f))
				{
					transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
				}
				else
				{
					Debug.Log("разворот");
					yield return new WaitForSeconds(3f);
					break;
				}
			}
			else if (dir.x == -1)
			{
				Ray rayL = new Ray(transform.position, Vector3.left);
				if (Physics.Raycast(rayL, out hit, 1f))
				{
					transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
				}
				else
				{
					Debug.Log("разворот");
					yield return new WaitForSeconds(3f);
					break;
				}
			}
			
			//if (((Physics.Raycast(transform.position, Vector3.right, out hit, 1f) && dir.x == 1f) || (Physics.Raycast(transform.position, Vector3.left, out hit, 1f) && dir.x == -1f)) && hit.transform.gameObject.layer == 0)
			//{
			//}
			timer -= Time.deltaTime;
			yield return null;
		}
		dir.x *= -1f;
		yield return null;
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
		level = Mathf.Clamp(level+0.5f,1,5);
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

	public void StartMoveInRoom(RoomScript.Resources roomType, List<Vector3> points, GameObject obj)
	{
		StartCoroutine(MoveInRoom(roomType, points, obj));
	}

	public IEnumerator MoveInRoom(RoomScript.Resources roomType, List<Vector3> walkPoints, GameObject obj)
	{
		Vector3 chosenPoint;
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
				while (obj.GetComponent<RoomScript>().status == RoomScript.Status.Busy)
				{
					yield return null;
				}
				StopCoroutine(walkingCoroutine);
				walkingCoroutine = null;
				break;
		}
	}

	private IEnumerator BedroomWalkBehaviour(List<Vector3> walkPoints, GameObject obj)
	{
		Vector3 chosenPoint;
		//State = States.Walk;
		chosenPoint = walkPoints[Random.Range(0, walkPoints.Count - 1)];
		while (!(chosenPoint.x - 0.01f <= transform.position.x && transform.position.x <= chosenPoint.x + 0.01f))
		{
			transform.Translate(new Vector3(Mathf.Sign(chosenPoint.x - transform.position.x), 0, 0) * Time.deltaTime);
			yield return null;
		}
		// move hands "working"
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

    public enum States
    {
        Idle,
        Walk,
        Working
    }
}
