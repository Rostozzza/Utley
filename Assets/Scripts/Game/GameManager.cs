using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
	public static RequestManager RequestManager = new RequestManager();

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
	/// ���������� ������� �������� ��� �� �������.
	/// </summary>
	/// <returns></returns>
	public int GetHoney()
	{
		return honey;
	}
	
	/// <summary>
	/// ���������� ������� �������� ������� �� �������.
	/// </summary>
	/// <returns></returns>
	public int GetAsteriy()
	{
		return asteriy;
	}

	/// <summary>
	/// �������� ���������� ���.
	/// </summary>
	/// <param name="amount"></param>
	public void ChangeHoney(int amount)
	{
		honey += amount;
	}

	/// <summary>
	/// �������� ���������� �������.
	/// </summary>
	/// <param name="amount"></param>
	public void ChangeAsteriy(int amount)
	{
		asteriy += amount;
	}
}
