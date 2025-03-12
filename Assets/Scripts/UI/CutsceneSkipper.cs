using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneSkipper : MonoBehaviour
{
    private Image fill;
    private GameObject text;
    private Vector2 textStartPos;
    private float amount;
    private float valueForSin = 0;
    [SerializeField] private float blinkSpeed = 1;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>().gameObject;
        fill = GetComponent<Image>();
        fill.fillAmount = 0;
        textStartPos = text.transform.localPosition;
    }

    private IEnumerator FillHolder(float timer)
    {
        float holdTimer = 0;
        while (true)
        {
            amount = holdTimer / timer;
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
                fill.fillAmount = amount;
                TextShake(amount * 2);
            }
            else
            {
                MenuManager.Instance.SkipCutscene();
                MenuManager.Instance.ClearSkipChecker();
                fill.fillAmount = 0;
                TextShake(0);
                yield break;
            }
            TextBlink();
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
        text.SetActive(set);
    }

    private void TextShake(float intensity)
    {
        text.transform.localPosition = textStartPos + intensity * Random.insideUnitCircle;
    }

    private void TextBlink()
    {
        if (amount <= 0)
        {
            text.GetComponent<TextMeshProUGUI>().alpha = (Mathf.Sin(valueForSin) + 1) / 2;
        }
        else
        {
            if (text.GetComponent<TextMeshProUGUI>().alpha != 1) text.GetComponent<TextMeshProUGUI>().alpha = 1;
        }

        valueForSin += Time.deltaTime * blinkSpeed;

        if (valueForSin > 2 * Mathf.PI)
        {
            valueForSin = 0;
        }
    }
}
