using System.Collections.Generic;
using UnityEngine;
using API.Sevices.Mapper;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
	public static RequestManager RequestManager = new RequestManager();
	public List<GameObject> bears = new List<GameObject>();
	[SerializeField] private GameObject selectedUnit;

	private int honey;
	private int asteriy;

	RaycastHit raycastHit;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
	
	/// <summary>
	/// Returns amount of honey on current client
	/// </summary>
	/// <returns></returns>
	public int GetHoney()
	{
		return honey;
	}
	
	/// <summary>
	/// Returns amount of Asteriun on current client
	/// </summary>
	/// <returns></returns>
	public int GetAsteriy()
	{
		return asteriy;
	}

	/// <summary>
	/// Changes amount of honey by given number
	/// </summary>
	/// <param name="amount"></param>
	public void ChangeHoney(int amount)
	{
		honey += amount;
	}

	/// <summary>
	/// Changes amount of asterium by given number
	/// </summary>
	/// <param name="amount"></param>
	public void ChangeAsteriy(int amount)
	{
		asteriy += amount;
	}

	private void Update()
	{
		InputHandler();
	}

	private void InputHandler()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out raycastHit, 100f))
			{
				if (raycastHit.transform != null)
				{
					ClickedGameObject(raycastHit.transform.gameObject);
				}
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out raycastHit, 100f))
			{
				if (raycastHit.transform != null)
				{
					ClickedGameObject(raycastHit.transform.gameObject);
				}
			}
		}
	}

	private void RightClick(GameObject gameObject)
	{
		if (gameObject.CompareTag("room"))
		{
			selectedUnit.GetComponent<UnitMovement>().StopAllCoroutines();
			selectedUnit.GetComponent<UnitMovement>().MoveToRoom(gameObject.GetComponent<RoomScript>());
		}
	}

	private void ClickedGameObject(GameObject gameObject)
	{
		if (gameObject.CompareTag("unit"))
		{
			//gameObject.GetComponent<UnitScript>().ChooseUnit();
			selectedUnit = gameObject;
		}
		else
		{
			selectedUnit = null;
		}
	}
}
