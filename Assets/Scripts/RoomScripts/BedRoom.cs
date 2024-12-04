using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BedRoom : RoomScript
{
	protected override IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		statusPanel.UpdateStatus(status);
		fixedBear.GetComponent<UnitScript>().SetBusy(true);
		fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
		animator.SetTrigger("StartWork");
		//!borrowed part!//
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.creator)
		{
			timer = 45f * (1 - 0.25f * (level - 1)) * (1 - (Mathf.FloorToInt(fixedBear.GetComponent<UnitScript>().level) * 0.5f));
		}
		else
		{
			timer = 45f * 1.25f * (1 - 0.25f * (level - 1));
		}
		fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Bed, GetWalkPoints(), this.gameObject);
		timer = 120f;
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
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.creator)
		{
			fixedBear.GetComponent<UnitScript>().LevelUpBear();
		}
		timeShow.text = "";
		GameManager.Instance.BoostThreeBears();
		fixedBear.GetComponent<UnitScript>().SetBusy(false);
		//!borrowed part!//
		if (fixedBear != null)
		{
			fixedBear.GetComponent<UnitScript>().CanBeSelected();
			fixedBear = null;
		}
		status = Status.Free;
		statusPanel.UpdateStatus(status);
		fixedBear.GetComponent<UnitScript>().SetWorkStr("Не занят");
		animator.SetTrigger("EndWork");
		audioSource.Stop();
	}
}
