using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnergohoneyRoom : RoomScript
{
	[SerializeField] GameObject setPipesButtonScreen;
	[SerializeField] public List<Renderer> renderersToChangeMaterialOnEvent;
	[SerializeField] public List<GameObject> pipes;
	

    protected override void Start()
    {
		pipes[(SceneManager.GetActiveScene().buildIndex == 4) ? 0 : 1].SetActive(false); 
        base.Start();
    }

	public override void ShowButton()
	{
		if (isEnpowered && status == Status.Free && durability > 0 && CheckIfSolved())
		{
			assignmentButton.SetActive(true);
		}
	}

	public override void HideButton()
	{
		if (resource != Resources.Asteriy )
		{
			assignmentButton.SetActive(false);
		}
	}

	public void GiveAnswerToExercise(string answer)
	{
		MenuManager.Instance.numberSummation.GiveAnswer(int.Parse(answer));
	}

	protected override IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		statusPanel.UpdateStatus(status);
		fixedBear.GetComponent<UnitScript>().SetBusy(true);
		fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
		animator.SetTrigger("StartWork");
		TrySetVideoPlayers(true);
		fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		//!borrowed part!//
		fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Energohoney, GetWalkPoints(), this.gameObject);
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.beekeeper && (level != 1 && fixedBear.GetComponent<UnitScript>().level != 1))
		{
			//timer = 45f * (1 - 0.25f * (level - 1)) * (1 - 0.05f * fixedBear.GetComponent<UnitScript>().level);
			//timer = (StandartInteractionTime + 5) * (1 - (SpeedByBearLevelCoef - 1) * fixedBear.GetComponent<UnitScript>().level) * SpeedByUsingSuitableBearCoef * (level > 1 ? (1 - ( 1 - SpeedByRoomLevelCoef) * level) : 1);
			timer = GetModifiedInteractionTime(fixedBear.GetComponent<UnitScript>().level);
			fixedBear.GetComponent<UnitScript>().expParticle.SetActive(true);
			fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(true);
		}
		else
		{
			//timer = 45f * 1.25f * (1 - 0.25f * (level - 1));
			//timer = (StandartInteractionTime + 5) * (level > 1 ? (1 - ( 1 - SpeedByRoomLevelCoef) * level) : 1);
			timer = (float)ValuesHolder.StandartInteractionTime;
		}
		if (fixedBear.GetComponent<UnitScript>().isBoosted)
		{
			timer *= 0.9f;
		}
		if (GameManager.Instance.GetIsEventAIEnpowered()) timer *= 0.5f;
		int honeyToAdd = (GameManager.Instance.season != GameManager.Season.Storm) ? (int)ValuesHolder.EnergohoneyAmountByOneInteraction : (int)(ValuesHolder.EnergohoneyAmountByOneInteraction * (1 - 0.15f + 0.03f * (GameManager.Instance.cycleNumber * ValuesHolder.CycleModifier)));
		workUI.StartWork(timer, honeyToAdd, GameManager.Instance.uiResourceShower.energoHoneyAmountText.transform, RoomWorkUI.ResourceType.Energohoney);
		EventManager.onBearWorkStarted.Invoke(this);
		while (timer > 0)
		{
			//timeShow.text = SecondsToTimeToShow(timer);
			timer -= Time.deltaTime;
			yield return null;
		}
		//timeShow.text = "";
		
		GameManager.Instance.uiResourceShower.UpdateIndicators();
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.beekeeper)
		{
			fixedBear.GetComponent<UnitScript>().LevelUpBear();
		}
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
		animator.SetTrigger("EndWork");
		TrySetVideoPlayers(false);
		audioSource.Stop();
		GameManager.Instance.ChangeHoney(honeyToAdd, new Log
		{
			comment = $"Added {honeyToAdd} honey to player {GameManager.Instance.playerName} for working on apiary",
			player_name = GameManager.Instance.playerName,
			resources_changed = new System.Collections.Generic.Dictionary<string, float> { {"honey",honeyToAdd } },
			 
		});
		roomStatsController.RefreshDescription();
	}

	public override void SetPipes()
	{
		Camera.main.GetComponent<CameraController>().GoToTaskPoint(cameraPoint.position,cameraAngle,true);
		MenuManager.Instance.CallProblemSolver(MenuManager.ProblemType.SetPipes,this);
		HideSetPipesButtonScreen();
		MakeUICanvas();
	}

	public override bool CheckIfSolved() => isSolved;
	
	public void SetIsSolved(bool set) => isSolved = set;
}