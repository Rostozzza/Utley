using System.Collections.Generic;
using UnityEngine;

public class ElevatorButtonsDeactivate : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttons;
    void Start()
    {
        if (buttons.Count > 0)
        {
            foreach (var button in buttons)
            {
                try { button.SetActive(GameManager.Instance.mode == GameManager.Mode.Build); } catch {}
            }
        }
    }
}
