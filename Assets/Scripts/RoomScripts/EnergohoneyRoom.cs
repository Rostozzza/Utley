using UnityEngine;
using System.Collections;

public class EnergohoneyRoom : RoomScript
{
	protected override IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		statusPanel.UpdateStatus(status);
		fixedBear.GetComponent<UnitScript>().SetBusy(true);
		fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
		animator.SetTrigger("StartWork");
		fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		//!borrowed part!//
		fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Energohoney, GetWalkPoints(), this.gameObject);
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.beekeeper)
		{
			timer = 45f * (1 - 0.25f * (level - 1)) * (1 - 0.05f * fixedBear.GetComponent<UnitScript>().level);
			fixedBear.GetComponent<UnitScript>().expParticle.SetActive(true);
			fixedBear.GetComponent<UnitScript>().GetStatusPanel().UpdateLoveWork(true);
		}
		else
		{
			timer = 45f * 1.25f * (1 - 0.25f * (level - 1));
		}
		if (fixedBear.GetComponent<UnitScript>().isBoosted)
		{
			timer *= 0.9f;
		}
		while (timer > 0)
		{
			timeShow.text = SecondsToTimeToShow(timer);
			timer -= Time.deltaTime;
			yield return null;
		}
		timeShow.text = "";

		int honeyToAdd = (GameManager.Instance.season != GameManager.Season.Storm) ? 10 : (int)(10 * (1 - 0.15f + 0.03f * GameManager.Instance.cycleNumber));
		GameManager.Instance.ChangeHoney(honeyToAdd);
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
		audioSource.Stop();
	}
}
