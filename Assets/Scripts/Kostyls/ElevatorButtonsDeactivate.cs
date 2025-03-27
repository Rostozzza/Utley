using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorButtonsDeactivate : MonoBehaviour
{
    [SerializeField] private Material corruptedByAI;
    [SerializeField] private List<Renderer> toReplaceByAIEvent;
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

        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            foreach (var rend in toReplaceByAIEvent)
            {
                rend.material = corruptedByAI;
            }
        }
    }
}
