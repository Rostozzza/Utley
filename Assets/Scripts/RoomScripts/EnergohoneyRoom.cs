
using UnityEngine;
using System.Collections;

public class EnergohoneyRoom : RoomScript
{
	protected override IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		//!borrowed part!//
		fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Energohoney, GetWalkPoints(), this.gameObject);
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.beekeeper)
		{
			timer = 45f * (1 - 0.25f * (level - 1)) * (1 - (Mathf.FloorToInt(fixedBear.GetComponent<UnitScript>().level) * 0.5f));
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
		//!borrowed part!//
		if (fixedBear != null)
		{
			fixedBear.GetComponent<UnitScript>().CanBeSelected();
			fixedBear = null;
		}
		status = Status.Free;
	}
}
