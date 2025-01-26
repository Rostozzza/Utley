using System.Collections.Generic;
using UnityEngine;

public class RoomOutliner : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> outlineFrameParts;
    private Color color = Color.yellow;
    void Start()
    {
        if (outlineFrameParts.Count == 0)
        {
            Debug.Log("Outline frames is missing in " + name + "!!!");
        }
        else // things to do at start if all right
        {
            outlineFrameParts.ForEach(x => x.color = color);
            SetOutline(false);
        }
    }

    /// <summary>
    /// Shows or hides outline by bool (show - true / hide - false)
    /// </summary>
    /// <param name="set"></param>
    public void SetOutline(bool set)
    {
        foreach (var framePart in outlineFrameParts)
        {
            framePart.enabled = set;
        }
    }
}
