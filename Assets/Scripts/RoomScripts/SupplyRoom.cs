
using System.Collections;
using UnityEngine;

public class SupplyRoom : RoomScript
{
	protected override IEnumerator WorkStatus()
	{
		float timer;
		status = Status.Busy;
		animator.SetTrigger("StartWork");
		//!borrowed part!//
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.coder)
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
		if (fixedBear.GetComponent<UnitScript>().job == Qualification.coder)
		{
			fixedBear.GetComponent<UnitScript>().LevelUpBear();
		}
		timeShow.text = "";
		GameManager.Instance.BoostThreeBears();
		//!borrowed part!//
		if (fixedBear != null)
		{
			fixedBear.GetComponent<UnitScript>().CanBeSelected();
			fixedBear = null;
		}
		status = Status.Free;
		animator.SetTrigger("EndWork");
	}
}
