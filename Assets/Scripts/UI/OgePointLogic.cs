using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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

	public void ConnectPoints()
	{
		foreach (OgePointLogic point in connectedPoints)
		{
			var lineInstance = Instantiate(line, transform).GetComponent<LineRenderer>();
			lineInstance.useWorldSpace = true;
			lineInstance.SetPosition(0, transform.position);
			lineInstance.SetPosition(1, point.transform.position);
			if (!point.sourcePoints.Contains(this))
			{
				point.sourcePoints.Add(this);
			}
		}
	}

	public void SetIsUsing(bool set)
	{
		//GameManager.Instance.SetIsGraphUsing(set);
	}

	private void OnDestroy()
	{
		//GameManager.Instance.SetIsGraphUsing(false);
	}

	public void SetColor(Color color)
	{
		transform.GetComponent<Image>().color = color;
	}

	public List<OgePointLogic> GetConnectedPoints()
	{
		return connectedPoints;
	}

	public List<OgePointLogic> GetSourcePoints()
	{
		return sourcePoints;
	}
}
