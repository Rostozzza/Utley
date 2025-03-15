using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class BedRoom : RoomScript
{
    protected override IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		statusPanel.UpdateStatus(status);
		//fixedBear.GetComponent<UnitScript>().SetBusy(true);
		//fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
		//fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(false);
		animator.SetTrigger("StartWork");
		TrySetVideoPlayers(true);
		//!borrowed part!//
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.creator && (level != 1 && fixedBear.GetComponent<UnitScript>().level != 1))
		{
			//timer = 45f * (1 - 0.25f * (level - 1)) * (1 - 0.05f * fixedBear.GetComponent<UnitScript>().level);
			timer = GetModifiedInteractionTime(fixedBear.GetComponent<UnitScript>().level);
			fixedBear.GetComponent<UnitScript>().expParticle.SetActive(true);
			fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(true);
		}
		else
		{
			//timer = 45f * 1.25f * (1 - 0.25f * (level - 1));
			timer = ValuesHolder.StandartInteractionTime;
		}
		fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Bed, GetWalkPoints(), this.gameObject);
		timer = 150f;
		//if (fixedBear.GetComponent<UnitScript>().isBoosted)
		//{
		//	timer *= 0.9f;
		//}
		(workUI as FluidWorkUI).StartWork(timer, 20, GameManager.Instance.uiResourceShower.bearsAmountText.transform);
		while (timer > 0)
		{
			//timeShow.text = SecondsToTimeToShow(timer);
			timer -= Time.deltaTime;
			yield return null;
		}
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.creator)
		{
			fixedBear.GetComponent<UnitScript>().LevelUpBear();
		}
		timeShow.text = "";
		GameManager.Instance.BoostThreeBears();
		//fixedBear.GetComponent<UnitScript>().SetBusy(false);
		//fixedBear.GetComponent<UnitScript>().SetWorkStr("Не занят");
		//!borrowed part!//
		if (fixedBear != null)
		{
			fixedBear.GetComponent<UnitScript>().SetWorkStr("Не занят");
			fixedBear.GetComponent<UnitScript>().CanBeSelected();
			fixedBear = null;
		}
		status = Status.Free;
		statusPanel.UpdateStatus(status);
		animator.SetTrigger("EndWork");
		TrySetVideoPlayers(false);
		audioSource.Stop();
		roomStatsController.RefreshDescription();
	}
}
