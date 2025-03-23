using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Threading.Tasks;

public class SupplyRoom : RoomScript
{
	[SerializeField] List<GameObject> graphs;
	private TMP_InputField currentAnswerField;
	public bool isSoft = false;
	GameObject graph;
	[SerializeField] private List<GameObject> poweredRooms;

	protected override void Start()
	{
		isEnpowered = true;
		base.Start();
		if (isSoft)
		{
			GetRoomsToEnpower();
		}
	}
	public override void SetPipes()
	{
		MenuManager.Instance.CallProblemSolver(MenuManager.ProblemType.SetSupply, this);
		Camera.main.GetComponent<CameraController>().GoToTaskPoint(cameraPoint.position, cameraAngle, true);
		HideSetPipesButtonScreen();
		MakeUICanvas();
	}

	public void GetRoomsToEnpower()
	{
		var horizontalRooms = GameManager.Instance.allRooms.Where(x => Mathf.Abs(x.transform.position.x - transform.position.x) <= 9f //17f earlier range was longer;
																	&& x.transform.position.y == transform.position.y && x.GetComponent<RoomScript>()).ToList();
		var verticalRooms = GameManager.Instance.allRooms.Where(x => Mathf.Abs(x.transform.position.y - transform.position.y) <= 5f //9f
																	&& x.transform.position.x == transform.position.x && x.GetComponent<RoomScript>()).ToList();
		var diagonalRooms = GameManager.Instance.allRooms.Where(x => Mathf.Abs(x.transform.position.x - transform.position.x) <= 9f
																	&& Mathf.Abs(x.transform.position.y - transform.position.y) <= 5f && x.GetComponent<RoomScript>()).ToList();
		poweredRooms = horizontalRooms;
		poweredRooms.AddRange(verticalRooms);
		poweredRooms.AddRange(diagonalRooms);
		foreach (var room in poweredRooms.Distinct().ToList())
		{
			if (room.GetComponent<RoomScript>() == this)
			{
				continue;
			}
		}
		foreach (var room in poweredRooms.Distinct().ToList())
		{
			if (room.GetComponent<RoomScript>() == this)
			{
				continue;
			}
			//Debug.Log($"Trying to empower {room.name}");
			room.GetComponent<RoomScript>().Enpower();
		}
		try
		{
			Destroy(graph);
		}
		catch { }
	}

    public override void Enpower()
    {
		isEnpowered = true;
		ChangeDurability(0);
    }

    public void GetRoomsToUnpower()
	{
		var horizontalRooms = GameManager.Instance.allRooms.Where(x => Mathf.Abs(x.transform.position.x - transform.position.x) <= 9f //17f
																	&& x.transform.position.y == transform.position.y && x.GetComponent<RoomScript>()).ToList();
		var verticalRooms = GameManager.Instance.allRooms.Where(x => Mathf.Abs(x.transform.position.y - transform.position.y) <= 5f //9f
																	&& x.transform.position.x == transform.position.x && x.GetComponent<RoomScript>()).ToList();
		var diagonalRooms = GameManager.Instance.allRooms.Where(x => Mathf.Abs(x.transform.position.x - transform.position.x) <= 9f
																	&& Mathf.Abs(x.transform.position.y - transform.position.y) <= 5f && x.GetComponent<RoomScript>()).ToList();
		poweredRooms = horizontalRooms;
		poweredRooms.AddRange(verticalRooms);
		poweredRooms.AddRange(diagonalRooms);

		List<SupplyRoom> supplyRooms = GameManager.Instance.allRooms.Where(x => x.GetComponent<SupplyRoom>()).Select(x=>x.GetComponent<SupplyRoom>()).Distinct().ToList();

		poweredRooms = poweredRooms.Distinct().ToList();

		foreach (var supplyRoom in supplyRooms)
		{
			if (poweredRooms.Contains(supplyRoom.gameObject)) poweredRooms.Remove(supplyRoom.gameObject);
		}

		foreach (var room in poweredRooms.Distinct())
		{
			if (room.GetComponent<RoomScript>() == this)
			{
				continue;
			}
			Debug.Log(room.name);
		}
		foreach (var room in poweredRooms.Distinct().Where(x => x != gameObject).Select(x => x.GetComponent<RoomScript>()).ToList())
		{
			if (room.GetComponent<RoomScript>() == this)
			{
				continue;
			}
			room.Unpower();
		}
	}

	public override void ChangeDurability(float hp)
	{
		bool isUnempoweredRooms = false;
		if (durability <= hp)
		{
			GetRoomsToUnpower();
			Debug.Log("<color=red>ПЫТАЕМСЯ ОБЕСТОЧИТЬ КОМНАТЫ</color>");
			isUnempoweredRooms = true;
		}
		base.ChangeDurability(hp);
		if (durability <= 0 && !isUnempoweredRooms)
		{
			GetRoomsToUnpower();
			Debug.Log("<color=red>ПЫТАЕМСЯ ОБЕСТОЧИТЬ КОМНАТЫ</color>");
		}
	}

	public void InitializeGraph()
	{
		int randNum = Random.Range(0, graphs.Count);
		graph = graphs[randNum];
		currentAnswerField = graph.GetComponentsInChildren<TMP_InputField>().First(x => x.gameObject.tag == "answerField");
		graph.SetActive(true);
		Time.timeScale = 1;
	}

	public void SolveGraph(int answer)
	{
		try
		{
			if (answer == int.Parse(currentAnswerField.text.ToString()))
			{
				GetRoomsToEnpower();
			}
			else
			{
				StartCoroutine(BlinkRed());
			}
		}
		catch
		{
			//StartCoroutine(BlinkRed());
		}
	}

	private IEnumerator BlinkRed()
	{
		var inputFields = graph.GetComponentsInChildren<TMP_InputField>();
		Debug.Log(inputFields.Length);
		for (int i = 0; i < 10; i++)
		{
			foreach (var inputField in inputFields)
			{
				inputField.textComponent.color = Color.red;
			}
			yield return new WaitForSeconds(0.1f);
			foreach (var inputField in inputFields)
			{
				inputField.textComponent.color = Color.white;
			}
			yield return new WaitForSeconds(0.1f);

		}
		inputFields.Select(x => x.text = "");
		yield return null;
	}

	protected override IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		statusPanel.UpdateStatus(status);
		fixedBear.GetComponent<UnitScript>().SetBusy(true);
		animator.SetTrigger("StartWork");
		fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		//!borrowed part!//
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.coder && (level != 1 && fixedBear.GetComponent<UnitScript>().level != 1))
		{
			//timer = 45f * (1 - 0.25f * (level - 1)) * (1 - 0.05f * fixedBear.GetComponent<UnitScript>().level);
			timer = GetModifiedInteractionTime(fixedBear.GetComponent<UnitScript>().level);
			fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(true);
		}
		else
		{
			//timer = 45f * 1.25f * (1 - 0.25f * (level - 1));
			timer = ValuesHolder.StandartInteractionTime;
		}
		fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Supply, GetWalkPoints(), this.gameObject);
		timer = 120f;
		if (fixedBear.GetComponent<UnitScript>().isBoosted)
		{
			timer *= 0.9f;
		}
		while (timer > 0)
		{
			//timeShow.text = SecondsToTimeToShow(timer);
			timer -= Time.deltaTime;
			yield return null;
		}
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.coder)
		{
			fixedBear.GetComponent<UnitScript>().LevelUpBear();
		}
		//timeShow.text = "";
		fixedBear.GetComponent<UnitScript>().SetBusy(false);
		//!borrowed part!//
		fixedBear.GetComponent<UnitScript>().SetWorkStr("Не занят");
		if (fixedBear != null)
		{
			fixedBear.GetComponent<UnitScript>().CanBeSelected();
			fixedBear = null;
		}
		status = Status.Free;
		statusPanel.UpdateStatus(status);
		fixedBear.GetComponent<UnitScript>().CanBeSelected();
		animator.SetTrigger("EndWork");
		fixedBear.GetComponent<UnitScript>().SetBusy(false);
		audioSource.Stop();
		roomStatsController.RefreshDescription();
	}
}
