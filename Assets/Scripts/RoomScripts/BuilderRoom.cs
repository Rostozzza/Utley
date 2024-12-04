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
    }

    protected override IEnumerator WorkStatus()
    {
		float timer;
		status = Status.Busy;
        SetWait(true);
		statusPanel.UpdateStatus(status);
		animator.SetTrigger("StartWork");
		fixedBear.GetComponent<UnitScript>().StartMoveInRoom(Resources.Build, new List<Vector3>(), this.gameObject);
        fixedBear.GetComponent<UnitScript>().CannotBeSelected();

        while (wait)
        {
            yield return null;
        }
        
		status = Status.Free;
        fixedBear.GetComponent<UnitScript>().CannotBeSelected();
		statusPanel.UpdateStatus(status);
		animator.SetTrigger("EndWork");
		audioSource.Stop();
    }
    
    public void SetWait(bool set)
    {
        wait = set;
    }

    public bool GetWait()
    {
        return wait;
    }
}
