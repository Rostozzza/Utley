using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIResourceShower : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI energoHoneyAmountText;
    [SerializeField] public TextMeshProUGUI asteriyAmountText;
    [SerializeField] public TextMeshProUGUI bearsAmountText;
    [SerializeField] public TextMeshProUGUI astroluminiteAmountText;
    [SerializeField] public TextMeshProUGUI ursowaksAmountText;
    [SerializeField] public TextMeshProUGUI prototypeAmountText;
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private GameObject honeyReducePanel;
    [SerializeField] private GameObject temperaturePanel;
    [SerializeField] private GameObject honeyReduceDynamic;
    [SerializeField] private GameObject temperatureDynamic;
    [SerializeField] private GameObject temperatureDynamic2;
    [SerializeField] private GameObject asteriumPanel;
    [SerializeField] private GameObject bearPanel;
    [SerializeField] private GameObject protypePanel;
    [SerializeField] private GameObject astroluminitePanel;
    [SerializeField] private GameObject ursowaksPanel;
    [SerializeField] private GameObject seasonPanel;
    [SerializeField] private TextMeshProUGUI seasonPanelText;
    [SerializeField] private GameObject seasonDebuffPanel;
    [SerializeField] private GameObject seasonDebuffPanel1;
    [SerializeField] private GameObject seasonDebuffPanel2;
    [SerializeField] private GameObject seasonDebuffPanel3;
    [SerializeField] private TextMeshProUGUI seasonDebuffPanelText;
    [SerializeField] private GameObject timeLeftPanel;
    private Coroutine seasonUpdater;
    [SerializeField] private Animator temperatureAnimator;
    [SerializeField] private GameObject temperatureTextToShake;

    [Header("Bars with info")]
    [SerializeField] private Image icon;
    [SerializeField] private Image iconDebuff;
    [SerializeField] private TextMeshProUGUI seasonHeader;
    [SerializeField] private TextMeshProUGUI seasonTimeLeft;
    [SerializeField] private TextMeshProUGUI seasonDiscription;
    [SerializeField] private List<Sprite> icons;
    [SerializeField] private List<Sprite> iconsDebuff;
    [SerializeField] private TextMeshProUGUI timeLeft;
    [SerializeField] private GameObject AIBoostShow;

	private void Start()
	{
		UpdateIndicators();
		honeyReducePanel.SetActive(false);
		temperaturePanel.SetActive(false);
		asteriumPanel.SetActive(false);
		bearPanel.SetActive(false);
        try
        {
            AIBoostShow.SetActive(false);
        }
        catch { }
		StartCoroutine(TimeChanger());
		StartCoroutine(TemperatureChanger());
	}

	/// <summary>
	/// Updates UI indicators (honey, asteriy, etc.)
	/// </summary>
	public void UpdateIndicators()
	{
		energoHoneyAmountText.text = Convert.ToString(Mathf.CeilToInt(GameManager.Instance.honey));
		asteriyAmountText.text = Convert.ToString(GameManager.Instance.asteriy);
		astroluminiteAmountText.text = Convert.ToString(Mathf.CeilToInt(GameManager.Instance.astroluminite));
		ursowaksAmountText.text = Convert.ToString(Mathf.CeilToInt(GameManager.Instance.ursowaks));
		prototypeAmountText.text = Convert.ToString(Mathf.CeilToInt(GameManager.Instance.prototype));
		bearsAmountText.text = Convert.ToString(GameManager.Instance.bears.Count) + "/" + Convert.ToString(GameManager.Instance.maxBearsAmount);
	}

    public void UpdateBarsStatuses()
    {
        icon.sprite = icons[(int)GameManager.Instance.season];
        seasonHeader.text = SeasonToHeaderText(GameManager.Instance.season);
        //seasonTimeLeft.text = "Ещё " + Convert.ToString((int)GameManager.Instance.GetSeasonTimeLeft()) + " с.";
        //seasonDiscription.text = SeasonToDiscriptionText(GameManager.Instance.season);
        iconDebuff.sprite = iconsDebuff[(int)GameManager.Instance.season];
    }

	private IEnumerator TemperatureChanger()
	{
        yield return null;
        bool isDecreasing = false;
		while (true)
		{
			temperatureSlider.value = (GameManager.Instance.GetTemperature() + (float)ValuesHolder.MinTemperature) / ((float)ValuesHolder.MaxTemperature + (float)ValuesHolder.MinTemperature);
			temperatureDynamic.GetComponent<TextMeshProUGUI>().text = Convert.ToString((int)GameManager.Instance.GetTemperature()) + " °C";
			temperatureDynamic2.GetComponent<TextMeshProUGUI>().text = Convert.ToString((int)GameManager.Instance.GetTemperature()) + " °C";
            
            if (GameManager.Instance.GetIsTemperatureDecreasing() && !isDecreasing) // temp starts decrease;
            {
                Debug.Log("Температура начала падать");
                isDecreasing = true;
                temperatureAnimator.SetTrigger("MakeAttention");
                SetVignette(0.3f, 0.25f, Color.blue);
                ShakeTemperature(2, 2);
            }
            if (!GameManager.Instance.GetIsTemperatureDecreasing() && isDecreasing)
            {
                Debug.Log("Температура начала расти");
                isDecreasing = false;
                temperatureAnimator.SetTrigger("MakeDefault");
                SetVignette(0, 0.25f, Color.blue);
            }

			yield return new WaitForSeconds(0.1f);
		}
	}

private void SetVignette(float set, float speed, Color color)
{
    GameManager.Instance.globalVolume.GetComponent<Volume>().profile.TryGet(out Vignette vignette);
    StartCoroutine(SmoothVignette(vignette, set, speed, color));
}

private IEnumerator SmoothVignette(Vignette vignette, float intense, float speed, Color color)
{
    vignette.color.value = color;
    while (vignette.intensity.value < intense - 0.01f || intense + 0.01f < vignette.intensity.value)
    {
        vignette.intensity.value += Mathf.Sign(intense - vignette.intensity.value) * Time.deltaTime * speed;
        yield return null;
    }
}

private void ShakeTemperature(float timer, float intensity)
{
    StartCoroutine(TemperatureShaker(timer, intensity));

    IEnumerator TemperatureShaker(float timer, float intensity)
    {
        Vector2 startPos = temperatureTextToShake.transform.localPosition;
        while (timer > 0)
        {
            temperatureTextToShake.transform.localPosition = startPos + Random.insideUnitCircle * intensity;
            timer -= Time.deltaTime;
            yield return null;
        }
    }
}

	private string SeasonToHeaderText(GameManager.Season season)
	{
		switch (season)
		{
			case GameManager.Season.Calm:
				return "Спокойная фаза";
			case GameManager.Season.Storm:
				return "Буревая фаза";
			case GameManager.Season.Freeze:
				return "Морозная фаза";
			case GameManager.Season.Tide:
				return "Приливная фаза";
			default:
				return "Неизвестная фаза";
		}
	}

    private IEnumerator TimeChanger()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            seasonTimeLeft.text = "Ещё " + Convert.ToString((int)GameManager.Instance.GetSeasonTimeLeft()) + " с.";
            //timeLeft.text = "Время до конца\nсмены: " + Convert.ToString((int)GameManager.Instance.GetTimeLeft()) + " с.";
            timeLeft.text = Convert.ToString((int)GameManager.Instance.GetTimeLeft()) + " с.";
            yield return null;
        }
    }

    private string SeasonToDiscriptionText(GameManager.Season season)
    {
        switch (season)
        {
            case GameManager.Season.Calm:
                return "Эффектов нет";
            case GameManager.Season.Storm:
                return $"Снижение выработки энергомеда на {15 + 3 * (GameManager.Instance.cycleNumber * ValuesHolder.CycleModifier)}%";
            case GameManager.Season.Freeze:
                return $"Увеличение потребления энергомеда\nна {10 + 5 * (GameManager.Instance.cycleNumber * ValuesHolder.CycleModifier)}%\nУвеличение скорости падения температуры\nна 25%";
            case GameManager.Season.Tide:
                return "Невозможность отправить космический корабль\nБыстрая потеря прочности у комплексов";
            default:
                return "Неизвестная фаза";
        }
    }

    public void ShowHint(PointerHint.HintType hintType)
    {
        switch (hintType)
        {
            case PointerHint.HintType.Energohoney:
                float honeyToEat = GameManager.Instance.CalculateHoneyToEat(false);
                honeyReducePanel.SetActive(true);
                honeyReduceDynamic.GetComponent<TextMeshProUGUI>().text = Convert.ToString((int)honeyToEat) + " в минуту";
                break;
            case PointerHint.HintType.Temperature:
                temperaturePanel.SetActive(true);
                temperatureDynamic.GetComponent<TextMeshProUGUI>().text = Convert.ToString((int)GameManager.Instance.GetTemperature()) + " °C";
                break;
            case PointerHint.HintType.Asterium:
                asteriumPanel.SetActive(true);
                break;
            case PointerHint.HintType.Bear:
                bearPanel.SetActive(true);
                break;
            case PointerHint.HintType.Ursowaks:
                ursowaksPanel.SetActive(true);
                break;
            case PointerHint.HintType.Prototype:
                protypePanel.SetActive(true);
                break;
            case PointerHint.HintType.Astroluminite:
                astroluminitePanel.SetActive(true);
                break;
            case PointerHint.HintType.Season:
                seasonPanelText.text = SeasonToText(GameManager.Instance.season);
                seasonUpdater = StartCoroutine(SeasonToTextUpdater());
                seasonPanel.SetActive(true);
                break;
            case PointerHint.HintType.SeasonDebuff:
                switch (GameManager.Instance.season)
                {
                    case GameManager.Season.Calm:
                        seasonDebuffPanel.GetComponentInChildren<TextMeshProUGUI>().text = SeasonToDiscriptionText(GameManager.Instance.season);
                        seasonDebuffPanel.SetActive(true);
                        break;
                    case GameManager.Season.Storm:
                        seasonDebuffPanel1.GetComponentInChildren<TextMeshProUGUI>().text = SeasonToDiscriptionText(GameManager.Instance.season);
                        seasonDebuffPanel1.SetActive(true);
                        break;
                    case GameManager.Season.Freeze:
                        seasonDebuffPanel2.GetComponentInChildren<TextMeshProUGUI>().text = SeasonToDiscriptionText(GameManager.Instance.season);
                        seasonDebuffPanel2.SetActive(true);
                        break;
                    case GameManager.Season.Tide:
                        seasonDebuffPanel3.GetComponentInChildren<TextMeshProUGUI>().text = SeasonToDiscriptionText(GameManager.Instance.season);
                        seasonDebuffPanel3.SetActive(true);
                        break;
                }
                //seasonDebuffPanelText.text = SeasonToDiscriptionText(GameManager.Instance.season);
                //seasonDebuffPanel.SetActive(true);
                break;
            case PointerHint.HintType.TimeLeft:
                timeLeftPanel.SetActive(true);
                break;
        }
    }

    public void HideHint(PointerHint.HintType hintType)
    {
        switch (hintType)
        {
            case PointerHint.HintType.Energohoney:
                honeyReducePanel.SetActive(false);
                break;
            case PointerHint.HintType.Temperature:
                temperaturePanel.SetActive(false);
                break;
            case PointerHint.HintType.Asterium:
                asteriumPanel.SetActive(false);
                break;
            case PointerHint.HintType.Bear:
                bearPanel.SetActive(false);
                break;
            case PointerHint.HintType.Ursowaks:
                ursowaksPanel.SetActive(false);
                break;
            case PointerHint.HintType.Prototype:
                protypePanel.SetActive(false);
                break;
            case PointerHint.HintType.Astroluminite:
                astroluminitePanel.SetActive(false);
                break;
            case PointerHint.HintType.Season:
                seasonPanel.SetActive(false);
                StopCoroutine(SeasonToTextUpdater());
                break;
            case PointerHint.HintType.SeasonDebuff:
                seasonDebuffPanel.SetActive(false);
                seasonDebuffPanel1.SetActive(false);
                seasonDebuffPanel2.SetActive(false);
                seasonDebuffPanel3.SetActive(false);
                break;
            case PointerHint.HintType.TimeLeft:
                timeLeftPanel.SetActive(false);
                break;
        }
    }

    private string SeasonToText(GameManager.Season season)
    {
        string toReturn;
        toReturn = season switch
        {
            GameManager.Season.Calm => "Спокойная фаза",
            GameManager.Season.Storm => "Буревая фаза",
            GameManager.Season.Freeze => "Морозная фаза",
            GameManager.Season.Tide => "Приливная фаза",
            _ => "Неизвестная фаза",
        };
        toReturn += $"\nещё <color=yellow>{Mathf.CeilToInt(GameManager.Instance.GetSeasonTimeLeft())}</color> с.";
        return toReturn;
    }

    private IEnumerator SeasonToTextUpdater()
    {
        while (true)
        {
            seasonPanelText.text = SeasonToText(GameManager.Instance.season);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void SetAIBoost(bool set)
    {
        AIBoostShow.SetActive(set);
    }
}
