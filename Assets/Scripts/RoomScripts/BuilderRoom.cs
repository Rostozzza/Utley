using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderRoom : RoomScript
{
    [SerializeField] private bool wait = false;

    override protected void Start()
    {
        base.Start();
        GameManager.Instance.builderRooms.Add(this.gameObject);
        fixedBear = null;
    }

    protected override IEnumerator WorkStatus()
    {
		float timer;
		status = Status.Busy;
        SetWait(true);
		statusPanel.UpdateStatus(status);
		animator.SetTrigger("StartWork");
		fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Build, new List<Vector3>(), this.gameObject);
		fixedBear.GetComponent<UnitScript>().SetWorkStr(workStr);
        fixedBear.GetComponent<UnitScript>().CanBeSelected();

        while (wait)
        {
			HideButton();
			yield return null;
        }
        
		status = Status.Free;
        fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		statusPanel.UpdateStatus(status);
		//fixedBear.GetComponent<UnitScript>().SetWorkStr("Не занят");
		animator.SetTrigger("EndWork");
		audioSource.Stop();
        fixedBear.GetComponentInChildren<Animator>().SetBool("Work", false);
		roomStatsController.RefreshDescription();
        //ShowButton();
    }
    
    public void SetWait(bool set)
    {
        wait = set;
    }

    public void SetWait(bool set, bool needToLeave)
    {
        wait = set;
        if (needToLeave)
        {
            fixedBear.GetComponent<UnitScript>().CanBeSelected();
		    status = Status.Free;
		    fixedBear.GetComponent<UnitScript>().SetWorkStr("Не занят");
		    animator.SetTrigger("EndWork");
		    audioSource.Stop();
            InterruptWork();
        }
    }

    public bool GetWait()
    {
        return wait;
    }
}
