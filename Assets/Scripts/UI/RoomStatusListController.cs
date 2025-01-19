using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RoomStatusListController : MonoBehaviour
{
	public List<GameObject> rooms;
	[SerializeField] private GameObject panelPrefab;
	[SerializeField] private Transform content;
	[SerializeField] private bool isHide = false;
	public RoomStatusController CreateRoomStatus(RoomScript obj)
	{
		GameObject instantiateRoomPanel = Instantiate(panelPrefab, content);
		rooms.Add(instantiateRoomPanel);
		instantiateRoomPanel.GetComponent<RoomStatusController>().Init(obj.gameObject, ResourcesTypeToName(obj.resource), obj.durability, obj.status, obj.resource);
		return instantiateRoomPanel.GetComponent<RoomStatusController>();
	}

	public void ClearList()
	{
		foreach (var item in GetComponentsInChildren<RoomStatusController>())
		{
			Destroy(item.gameObject);
		}
	}
	private string ResourcesTypeToName(RoomScript.Resources resource)
	{
		switch (resource)
		{
			case RoomScript.Resources.Energohoney:
				return "Комплекс выработки энергомеда";
			case RoomScript.Resources.Asteriy:
				return "Комплекс переработки астерия";
			case RoomScript.Resources.Cosmodrome:
				return "Космодром";
			case RoomScript.Resources.Bed:
				return "Жилой комплекс";
			case RoomScript.Resources.Build:
				return "Комплекс строительства";
			case RoomScript.Resources.Supply:
				return "Комплекс снабжения";
			case RoomScript.Resources.Research:
				return "Комната исследования";
			default:
				return "Неизвестный тип";
		}
	}

	public void SwitchHideList()
	{
		isHide = !isHide;
		GetComponent<Animator>().SetTrigger(isHide ? "Hide" : "Show");
	}

	//private IEnumerator ListAnim(bool toHide)
	//{
	//	if (toHide)
	//	{
	//		while (transform.localScale.x > 0)
	//		{
	//			transform.localScale -= new Vector3(0.01f, 0, 0);
	//			yield return null;
	//		}
	//	}
	//	else
	//	{
	//		while (transform.localScale.x < 1)
	//		{
	//			transform.localScale += new Vector3(0.01f, 0, 0);
	//			yield return null;
	//		}
	//	}
	//	hidingAnimCoroutine = null;
	//}
}
