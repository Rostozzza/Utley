using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ResearchRoom : RoomScript
{
	[SerializeField] private GameObject researchSelectScreen;
	[SerializeField] private Type waitOption = Type.None;
	[SerializeField] private Sprite ursowaksSprite;
	[SerializeField] private Sprite prototypeSprite;
	private float haveAstroluminte;
	private int haveAsteriy;

	public override async void StartWork(GameObject bear)
	{
		if (status != Status.Destroyed && isEnpowered)
		{
			fixedBear = bear;
			fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
			fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(false);
			if (resource == Resources.Cosmodrome)
			{
				status = Status.Busy;
				statusPanel.UpdateStatus(status);
				fixedBear.GetComponent<UnitScript>().CannotBeSelected();
				return;
			}
			if (status == Status.Free)// && resource != Resources.Build)
			{
				StartCoroutine(WorkStatus());
				audioSource.Play();
			}
		}
	}

	private IEnumerator SelectOption()
	{
		waitOption = Type.None;
		researchSelectScreen.SetActive(true);
		while (waitOption == Type.None)
		{
			yield return null;
		}
		//StartCoroutine(WorkStatus());
	}

	public void SendOption(int option) // 1 or 2 (0 is None) // 1 - Ursowaks, 2 - Prototype
	{
		waitOption = (Type)option;
		researchSelectScreen.SetActive(false);
	}

	private async Task GetHaveResources()
	{
		haveAsteriy = await GameManager.Instance.GetAsteriy();
		haveAstroluminte = await GameManager.Instance.GetAstroluminite();
	}

	private async Task ChangeResources(int asteriy, float astroluminite, string creatingResource)
	{
		await GameManager.Instance.ChangeAsteriy(asteriy, new Log
		{
			comment = $"Consumed {asteriy} asterium from {GameManager.Instance.playerName} to manufacture {creatingResource}",
			 
			player_name = GameManager.Instance.playerName,
			resources_changed = new Dictionary<string, float> { {"asterium",asteriy } }
		});
		await GameManager.Instance.ChangeAstroluminite(astroluminite, new Log
		{
			comment = $"Consumed {astroluminite} astroluminite from {GameManager.Instance.playerName} to manufacture {creatingResource}",
			 
			player_name = GameManager.Instance.playerName,
			resources_changed = new Dictionary<string, float> { { "astroluminite", astroluminite } }
		});
	}

	protected override IEnumerator WorkStatus()
	{
		yield return SelectOption();

		yield return GetHaveResources();

		switch (waitOption)
		{
			case Type.Ursowaks:
				if (haveAstroluminte < 5)
				{
					yield break;
				}
				yield return ChangeResources(0, -5, "ursowaks");
				break;
			case Type.Prototype:
				if (haveAstroluminte < 3)
				{
					yield break;
				}
				yield return ChangeResources(0, -3, "ursowaks");
				break;
			default:
				break;
		}
		GameManager.Instance.uiResourceShower.UpdateIndicators();

		float timer;
		status = Status.Busy;
		statusPanel.UpdateStatus(status);
		fixedBear.GetComponent<UnitScript>().SetBusy(true);
		fixedBear.GetComponent<UnitScript>().StartResearching();
		fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
		animator.SetTrigger("StartWork");
		fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		//!borrowed part!//
		fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Research, GetWalkPoints(), this.gameObject);

		if (fixedBear.GetComponent<UnitScript>().job == Qualification.bioengineer)
		{
			//timer = 18f * (1 - 0.25f * (level - 1)) * (1 - 0.05f * fixedBear.GetComponent<UnitScript>().level);
			timer = StandartInteractionTime * (1 - (SpeedByBearLevelCoef - 1) * fixedBear.GetComponent<UnitScript>().level) * SpeedByUsingSuitableBearCoef * (level > 1 ? (1 - ( 1 - SpeedByRoomLevelCoef) * level) : 1);
			fixedBear.GetComponent<UnitScript>().expParticle.SetActive(true);
			fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(true);
		}
		else
		{
			timer = StandartInteractionTime * (level > 1 ? (1 - ( 1 - SpeedByRoomLevelCoef) * level) : 1);
			//timer = 18f * 1.25f * (1 - 0.25f * (level - 1));
		}
		if (fixedBear.GetComponent<UnitScript>().isBoosted)
		{
			timer *= 0.9f;
		}
		switch (waitOption)
		{
			case Type.Ursowaks:
				workUI.SetResultImage(ursowaksSprite);
				workUI.SetWorkUnitSprite(ursowaksSprite);
				workUI.StartWork(timer,1,GameManager.Instance.uiResourceShower.ursowaksAmountText.transform);
				break;
			case Type.Prototype:
				workUI.SetResultImage(prototypeSprite);
				workUI.SetWorkUnitSprite(prototypeSprite);
				workUI.StartWork(timer, 1, GameManager.Instance.uiResourceShower.prototypeAmountText.transform);
				break;
			default:
				break;
		}
		while (timer > 0)
		{
			timeShow.text = SecondsToTimeToShow(timer);
			timer -= Time.deltaTime;
			yield return null;
		}
		timeShow.text = "";

		switch (waitOption)
		{
			case Type.Ursowaks:
				GameManager.Instance.ChangeUrsowaks(1,new Log
				{
					comment = $"Player {GameManager.Instance.playerName} manufactured 1 ursowaks",
					 
					player_name = GameManager.Instance.playerName,
					resources_changed = new Dictionary<string, float> { { "ursowaks", 1 } }
				});
				break;
			case Type.Prototype:
				GameManager.Instance.ChangePrototype(1,new Log
				{
					comment = $"Player {GameManager.Instance.playerName} manufactured 1 prototype",
					 
					player_name = GameManager.Instance.playerName,
					resources_changed = new Dictionary<string, float> { { "prototype", 1 } }
				});
				break;
			default:
				break;
		}

		GameManager.Instance.uiResourceShower.UpdateIndicators();
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.bioengineer)
		{
			fixedBear.GetComponent<UnitScript>().LevelUpBear();
		}
		fixedBear.GetComponent<UnitScript>().SetBusy(false);
		fixedBear.GetComponent<UnitScript>().StopResearching();


		//!borrowed part!//
		fixedBear.GetComponent<UnitScript>().SetWorkStr("Не занят");
		if (fixedBear != null)
		{
			fixedBear.GetComponent<UnitScript>().CanBeSelected();
			fixedBear = null;
		}
		status = Status.Free;
		statusPanel.UpdateStatus(status);
		animator.SetTrigger("EndWork");
		audioSource.Stop();
	}

	enum Type
	{
		None,
		Ursowaks,
		Prototype
	}
}
