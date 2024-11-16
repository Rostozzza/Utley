using System.Collections.Generic;
using UnityEngine;
using API.Sevices.Mapper;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
	public static RequestManager RequestManager = new RequestManager();
	public List<GameObject> bears = new List<GameObject>;

	private int honey;
	private int asteriy;

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
}
