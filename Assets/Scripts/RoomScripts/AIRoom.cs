using System.Collections.Generic;
using UnityEngine;

public class AIRoom : RoomScript
{
    [SerializeField] int countOfEnpowers = 0;
    [SerializeField] List<Vector2> placesToSpawn;

    protected override void Start()
    {
        transform.position = placesToSpawn[Random.Range(0, placesToSpawn.Count)];
        base.Start();
        CheckPower();
    }

    public override void Enpower()
    {
		countOfEnpowers++;
        CheckPower();
    }

    public override void Unpower()
    {
        countOfEnpowers--;
        CheckPower();
    }

    private void CheckPower()
    {
        if (countOfEnpowers >= 2)
        {
            isEnpowered = true;
        }
        else
        {
            isEnpowered = false;
        }
        ChangeDurability(0);

        animator.speed = isEnpowered ? 1 : 0;

        GameManager.Instance.SetIsEventAIEnpowered(isEnpowered);

        if (isEnpowered)
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }

        GameManager.Instance.uiResourceShower.SetAIBoost(isEnpowered);
    }
}
