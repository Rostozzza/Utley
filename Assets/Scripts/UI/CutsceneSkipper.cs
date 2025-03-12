using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneSkipper : MonoBehaviour
{
    private Image fill;

    void Awake()
    {
        fill = GetComponent<Image>();
        fill.fillAmount = 0;
    }

    private IEnumerator FillHolder(float timer)
    {
        float holdTimer = 0;
        while (true)
        {
            if (holdTimer < timer)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    holdTimer += Time.deltaTime;
                }
                else if (holdTimer > 0)
                {
                    holdTimer -= Time.deltaTime / 2f;
                }
                fill.fillAmount = holdTimer / timer;
            }
            else
            {
                MenuManager.Instance.SkipCutscene();
                MenuManager.Instance.ClearSkipChecker();
                fill.fillAmount = 0;
                yield break;
            }
            yield return null;
        }
    }

    public Coroutine AllowSkipCoroutine(float timer)
    {
        SetFillEnabled(true);
        return StartCoroutine(FillHolder(timer));
    }

    public void SetFillEnabled(bool set)
    {
        fill.enabled = set;
    }
}
