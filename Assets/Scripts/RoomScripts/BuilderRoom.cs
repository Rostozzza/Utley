using UnityEngine;

public class BuilderRoom : RoomScript
{
    override protected void Start()
    {
        base.Start();
        GameManager.Instance.builderRooms.Add(this.gameObject);
    }
}
