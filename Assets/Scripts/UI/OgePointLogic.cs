using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class OgePointLogic : MonoBehaviour
{
	public bool isActive;
	[SerializeField] private List<OgePointLogic> connectedPoints;
	[SerializeField] private List<OgePointLogic> sourcePoints;

	//[SerializeField] private Sprite exploredSprite;
	[SerializeField] private GameObject line;

	public void Start()
	{
		ConnectPoints();
	}

	private void ConnectPoints()
	{
		foreach (OgePointLogic point in connectedPoints)
		{
			var lineInstance = Instantiate(line).GetComponent<LineRenderer>();
			lineInstance.useWorldSpace = true;
			lineInstance.SetPosition(0, transform.position);
			lineInstance.SetPosition(1, point.transform.position);
			point.sourcePoints.Add(this);
		}
	}
}
