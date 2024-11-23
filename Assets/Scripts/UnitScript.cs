using System.Collections;
using System.Collections.Generic;
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
	public int level;
<<<<<<< HEAD
	private List<GameObject> fur;
	public List<GameObject> up;
	public List<GameObject> down;
=======
	public Mesh fur;
	public Mesh top;
	public Mesh bottom;
	public bool isBoosted = false;
>>>>>>> f27a35a27bb4f6e0f92dad5afe06185b34b5f734

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		onLadder = laddersAmount > 0;

		//if (chased)
		//{
		//    Debug.Log(randomWalk);
		//    if (randomWalk != null)
		//    {
		//        StopCoroutine(randomWalk);
		//        randomWalk = null;
		//    }
		//    Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -20f);
		//    job = Jobs.None;
		//    rb.useGravity = !onLadder;
		//    dir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
		//    rb.linearVelocity = new Vector3(speed * dir.x, onLadder ? speed * dir.y : rb.linearVelocity.y, 0f);
		//    if (Input.GetKeyDown(KeyCode.E) && nearWorkStation != null)
		//    {
		//        isBusy = true;
		//        chased = false;
		//        job = (Jobs)nearWorkStation.GetComponent<WorkStationScript>().GetJob();
		//    }
		//    if (Input.GetKeyDown(KeyCode.Escape))
		//    {
		//        chased = false;
		//    }
		//}
		//else
		//{
		//    //if (randomWalk == null)
		//    //{
		//    //    randomWalk = StartCoroutine(WalkCycle());
		//    //    Debug.Log("стартовали корутину");
		//    //}
		//    rb.useGravity = !onLadder;
		//    rb.linearVelocity = new Vector3(dir.x, onLadder ? 0f : rb.linearVelocity.y, 0f);
		//
	}

	private IEnumerator WalkCycle()
	{
		RaycastHit hit;
		dir.x = Random.Range(0, 2) == 1 ? 1 : -1;
		while (!chased)
		{
			float timer = 7f + Random.value;
			while (timer > 0f)
			{
				//rb.linearVelocity = new Vector3(dir.x, onLadder ? 0f : rb.linearVelocity.y, 0f);
				// Uncomment if wanna see cool rays-detectors
				Debug.DrawRay(transform.position, Vector3.right, Color.yellow);
				Debug.DrawRay(transform.position, Vector3.left, Color.yellow);
				if (((Physics.Raycast(transform.position, Vector3.right, out hit, 1f) && dir.x == 1f) || (Physics.Raycast(transform.position, Vector3.left, out hit, 1f) && dir.x == -1f)) && hit.transform.gameObject.layer == 0)
				{
					Debug.Log("разворот");
					yield return new WaitForSeconds(3f);
					break;
				}
				timer -= Time.deltaTime;
				yield return null;
			}
			dir.x *= -1f;
			yield return null;
		}
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
		State = States.Walk;
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
		while (obj.GetComponent<RoomScript>().status == RoomScript.Status.Busy)
		{
            State = States.Walk;
			chosenPoint = walkPoints[Random.Range(0, walkPoints.Count - 1)];
			while (!(chosenPoint.x - 0.01f <= transform.position.x && transform.position.x <= chosenPoint.x + 0.01f))
			{
				transform.Translate(new Vector3(Mathf.Sign(chosenPoint.x - transform.position.x), 0, 0) * Time.deltaTime);
				yield return null;
			}
            State = States.Working;
			yield return new WaitForSeconds(5f);
            State = States.Walk;
			chosenPoint = walkPoints[3];
			while (!(chosenPoint.x - 0.01f <= transform.position.x && transform.position.x <= chosenPoint.x + 0.01f))
			{
				//dir.x = Mathf.Sign(chosenPoint.x - transform.position.x);
				transform.Translate(new Vector3(Mathf.Sign(chosenPoint.x - transform.position.x), 0, 0) * Time.deltaTime);
				yield return null;
			}
            State = States.Working;
			yield return new WaitForSeconds(3f);
		}
	}

    public enum States
    {
        Idle,
        Walk,
        Working
    }

    public States State
    {
        get { return (States)animator.GetInteger("state"); }
        set { animator.SetInteger("State", (int)value); }
    }
}
