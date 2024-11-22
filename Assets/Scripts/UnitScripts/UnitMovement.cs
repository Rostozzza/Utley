using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class UnitMovement : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private RoomScript target;
	[SerializeField] private Elevator currentElevator;
	public Coroutine currentRoutine;

	public void MoveToRoom(RoomScript target)
	{
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
			var result = GetBranch(targetElevator, currentElevator, branch);
			branch = (result.Count < branch.Count ? result : branch);
		}
		var finalBranch = new List<Elevator> { currentElevator };
		finalBranch.AddRange(branch);
		currentRoutine = StartCoroutine(Move(finalBranch));
	}

	private IEnumerator MoveByOne()
	{
		if (target.transform.position.x < transform.position.x)
		{
			while (target.transform.position.x < transform.position.x)
			{
				transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
				yield return null;
			}
		}
		else
		{
			while (target.transform.position.x > transform.position.x)
			{
				transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
				yield return null;
			}
		}
		currentRoutine = null;
	}

	private IEnumerator Move(List<Elevator> path)
	{
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
						transform.position = new Vector3(transform.position.x, room.transform.position.y, transform.position.z);
						Debug.Log("Breakage!");
						break;
					}
				}
			}
			if (e.transform.position.x < transform.position.x)
			{
				while (e.transform.position.x < transform.position.x)
				{
					transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
					yield return null;
				}
			}
			else
			{
				while (e.transform.position.x > transform.position.x)
				{
					transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
					yield return null;
				}
			}
			currentElevator = e;
		}
		transform.position = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
		if (target.transform.position.x < transform.position.x)
		{
			while (target.transform.position.x < transform.position.x)
			{
				transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
				yield return null;
			}
		}
		else
		{
			while (target.transform.position.x > transform.position.x)
			{
				transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
				yield return null;
			}
		}
		currentRoutine = null;
	}

	private List<Elevator> GetBranch(Elevator targetElevator, Elevator elevator, List<Elevator> branch)
	{
		if (elevator == targetElevator)
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
}
