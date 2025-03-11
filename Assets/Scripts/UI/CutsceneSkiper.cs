using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneSkiper : MonoBehaviour
{
    Image fill;
    [SerializeField] private float timer;

    void Awake()
    {
        fill = GetComponent<Image>();
        StartCoroutine(FillHolder(timer));
    }

    void Update()
    {
    }

    private IEnumerator FillHolder(float timer)
    {
        float holdTimer = 0;
        while (true)
        {
            if (holdTimer < timer)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    holdTimer += Time.deltaTime;
                }
                else if (holdTimer > 0)
                {
                    holdTimer -= Time.deltaTime / 2f;
                }
            }
            else
            {
                // skip cutscene;
            }
        }
    }
}
