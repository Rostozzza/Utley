using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private RoomScript target;
	[SerializeField] private Elevator currentElevator;
	private Coroutine currentRoutine;

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
			branch = (result.Count < branch.Count ? branch = result : branch);
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
	}

	private IEnumerator Move(List<Elevator> path)
	{
		currentElevator = path[0];
		foreach (Elevator e in path)
		{
			if (e != currentElevator)
			{
				foreach (var room in e.connectedRooms)
				{
					if (room.connectedElevators.Contains(currentElevator))
					{
						transform.position = new Vector3(transform.position.x, room.transform.position.y, transform.position.z);
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
	}

	private List<Elevator> GetBranch(Elevator targetElevator, Elevator elevator, List<Elevator> branch)
	{
		if (elevator == targetElevator)
		{
			return new List<Elevator> { elevator };
		}
		foreach (var i in elevator.connectedElevators)
		{
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
		Debug.Log(branch.Count);
		return null;
	}
}
