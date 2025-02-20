using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class UnitMovement : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private RoomScript target;
	[SerializeField] public Elevator currentElevator;
	public RoomScript currentRoom;
	[SerializeField] private float offsetX = 1.24f;
	public Coroutine currentRoutine;
	[SerializeField] private bool isWalkingToWork = false;
	private bool isTransitioning = false;

	public bool IsWalkingToWork()
	{
		return isWalkingToWork;
	}

	public void SetIsWalkingToWork(bool isWalkingToWork)
	{
		this.isWalkingToWork = isWalkingToWork;
	}

	public void MoveToRoom(RoomScript target)
	{
		if (isWalkingToWork)
		{
			return;
		}
		if (isTransitioning)
		{
			return;
		}
		List<Elevator> branch = new List<Elevator>();
		this.target = target;
		if (currentRoutine != null)
		{
			StopCoroutine(currentRoutine);
		}
		if (target.connectedElevators.Contains(currentElevator))
		{
			if (target.transform.position.y == transform.position.y)
			{
				currentRoutine = StartCoroutine(MoveByOne());
				return;
			}
			branch = new List<Elevator> { currentElevator };
			currentRoutine = StartCoroutine(Move(branch));
			return;
		}
		foreach (var targetElevator in target.connectedElevators)
		{
			foreach (var startElevator in currentRoom.connectedElevators)
			{
				Debug.Log("--------Pair:--------");
				var result = GetBranch(targetElevator, startElevator, new List<Elevator> { startElevator}).Distinct().ToList();
				result.ForEach(e => { Debug.Log(e.name); });
				branch = ((result.Count < branch.Count && result.Count > 0) || branch.Count == 0) ? result : branch;
			}
		}
		Debug.Log($"Selected brach:");
		branch.ForEach(e => { Debug.Log(e.name); });
		currentRoutine = StartCoroutine(Move(branch));
	}

	private IEnumerator MoveByOne()
	{
		GetComponentInChildren<Animator>().SetBool("Walk", true);
		//gameObject.GetComponent<UnitScript>().State = UnitScript.States.Walk;
		if (target.transform.position.x < transform.position.x)
		{
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, -90, 0);
			while (target.transform.position.x + 1.24f < transform.position.x)
			{
				transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);

				yield return null;
			}
		}
		else
		{
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 90, 0);
			while (target.transform.position.x + 1.24f > transform.position.x)
			{
				transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);

				yield return null;
			}
		}
		currentRoutine = null;
		GetComponentInChildren<Animator>().SetBool("Walk", false);
	}

	public void UpdateCurrentElevator()
	{
		try
		{
			currentElevator = currentRoom.connectedElevators[0];
		}
		catch
		{
			Debug.Log("Couldnt update current elevator for unit");
		}
	}


	private IEnumerator Move(List<Elevator> path)
	{
		GetComponentInChildren<Animator>().SetBool("Walk", true);
		Debug.Log($"Before distinction: {path.Count}");
		var newPath = path.Distinct().ToList();
		Debug.Log(newPath.Count);
		currentElevator = path[0];
		foreach (Elevator e in newPath)
		{
			if (e != currentElevator)
			{
				foreach (var room in e.connectedRooms)
				{
					if (room.connectedElevators.Contains(currentElevator))
					{
						var directionElevator = room.transform.position.y;
						GetComponentInChildren<Animator>().SetBool("Walk", false);
						GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 0, 0);
						if (!GetComponentInChildren<Animator>().GetBool("ClimbUp") && !GetComponentInChildren<Animator>().GetBool("ClimbDown"))
						{
							if (transform.position.y - target.transform.position.y > 0)
							{
								GetComponentInChildren<Animator>().SetBool("ClimbUp", true);
							}
							else
							{
								GetComponentInChildren<Animator>().SetBool("ClimbDown", true);
							}
							GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 0, 0);
							isTransitioning = true;
							yield return new WaitForSeconds(1.15f);
							isTransitioning = false;
						}
						GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 0, 0);
						while (Mathf.Abs(transform.position.y-room.transform.position.y) > 0.1f)
						{
							transform.Translate(new Vector3(0, (room.transform.position.y - transform.position.y) * 100, 0).normalized * Time.deltaTime * speed / 2f);
							yield return null;
						}
						transform.position = new Vector3(transform.position.x, room.transform.position.y, transform.position.z);
						GetComponentInChildren<Animator>().SetBool("ClimbDown", false);
						GetComponentInChildren<Animator>().SetBool("ClimbUp", false);
						isTransitioning = true;
						yield return new WaitForSeconds(1.28f);
						isTransitioning = false;
						Debug.Log("Breakage!");
						break;
					}
				}
			}
			//gameObject.GetComponent<UnitScript>().State = UnitScript.States.Walk;
			GetComponentInChildren<Animator>().SetBool("Walk", true);
			if (e.transform.position.x + 1.24f < transform.position.x)
			{
				GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, -90, 0);
				while (e.transform.position.x + 1.24f < transform.position.x)
				{
					transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
					yield return null;
				}
			}
			else
			{
				GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 90, 0);
				while (e.transform.position.x + 1.24f > transform.position.x)
				{
					transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
					yield return null;
				}
			}
			isTransitioning = true;
			while (0.698f > transform.position.z)
			{
				transform.Translate(new Vector3(0, 0, 1) * speed * Time.deltaTime);
				yield return null;
			}
			isTransitioning = false;
			currentElevator = e;
		}

		GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 0, 0);
		if (!GetComponentInChildren<Animator>().GetBool("ClimbUp") && !GetComponentInChildren<Animator>().GetBool("ClimbDown"))
		{
			GetComponentInChildren<Animator>().SetBool("Walk", false);
			if (transform.position.y - target.transform.position.y > 0)
			{
				GetComponentInChildren<Animator>().SetBool("ClimbUp", true);
			}
			else
			{
				GetComponentInChildren<Animator>().SetBool("ClimbDown", true);
			}
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 0, 0);
			isTransitioning = true;
			yield return new WaitForSeconds(1.15f);
			isTransitioning = false;
		}
		while (Mathf.Abs(transform.position.y - target.transform.position.y) > 0.1f)
		{
			transform.Translate(new Vector3(0, (target.transform.position.y - transform.position.y) * 100, 0).normalized * Time.deltaTime * speed / 2f);
			yield return null;
		}
		transform.position = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
		GetComponentInChildren<Animator>().SetBool("ClimbDown", false);
		GetComponentInChildren<Animator>().SetBool("ClimbUp", false);
		isTransitioning = true;
		yield return new WaitForSeconds(1.28f);
		isTransitioning = false;
		GetComponentInChildren<Animator>().SetBool("Walk", true);
		isTransitioning = true;
		while (-1 < transform.position.z)
		{
			transform.Translate(new Vector3(0, 0, -1) * speed * Time.deltaTime);
			yield return null;
		}
		isTransitioning = false;
		//gameObject.GetComponent<UnitScript>().State = UnitScript.States.Walk;
		if (target.transform.position.x + 1.24f < transform.position.x)
		{
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, -90, 0);
			while (target.transform.position.x + 1.24f < transform.position.x)
			{
				transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
				yield return null;
			}
		}
		else
		{
			GetComponentInChildren<Animator>().transform.eulerAngles = new Vector3(0, 90, 0);
			while (target.transform.position.x + 1.24f > transform.position.x)
			{
				transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
				yield return null;
			}
		}
		
		EventManager.onBearReachedDestination.Invoke(target);
		currentRoutine = null;
		GetComponentInChildren<Animator>().SetBool("Walk", false);
	}

	private List<Elevator> GetBranch(Elevator targetElevator, Elevator elevator, List<Elevator> branch)
	{
		if (elevator.gameObject == targetElevator.gameObject)
		{
			return new List<Elevator> { elevator };
		}
		foreach (var i in elevator.connectedElevators)
		{
			if (i == elevator)
			{
				continue;
			}
			if (i.connectedElevators.Contains(targetElevator))
			{
				branch.Add(i);
				branch.Add(targetElevator);
				return branch;
			}
			if (i.connectedElevators.Count >= 2)
			{
				var newBranch = branch;
				newBranch.Add(elevator);
				return GetBranch(targetElevator, i, newBranch);
			}
		}
		return null;
	}

	public RoomScript GetTarget()
	{
		return target;
	}
}
