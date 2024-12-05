using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceShower : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energoHoneyAmountText;
    [SerializeField] private TextMeshProUGUI asteriyAmountText;
    [SerializeField] private TextMeshProUGUI bearsAmountText;
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private GameObject honeyReducePanel;
    [SerializeField] private GameObject temperaturePanel;
    [SerializeField] private GameObject honeyReduceDynamic;
    [SerializeField] private GameObject temperatureDynamic;
    [SerializeField] private GameObject asteriumPanel;
    [SerializeField] private GameObject bearPanel;

    [Header("Bars with info")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI seasonHeader;
    [SerializeField] private TextMeshProUGUI seasonTimeLeft;
    [SerializeField] private TextMeshProUGUI seasonDiscription;
    [SerializeField] private List<Sprite> icons;
    [SerializeField] private TextMeshProUGUI timeLeft;

    private void Start()
    {
        UpdateIndicators();
        honeyReducePanel.SetActive(false);
        temperaturePanel.SetActive(false);
        asteriumPanel.SetActive(false);
        bearPanel.SetActive(false);
        StartCoroutine(TimeChanger());
        StartCoroutine(TemperatureChanger());
    }

    /// <summary>
    /// Updates UI indicators (honey, asteriy, etc.)
    /// </summary>
    public void UpdateIndicators()
    {
        energoHoneyAmountText.text = Convert.ToString(Mathf.CeilToInt(GameManager.Instance.GetHoney().Result));
        asteriyAmountText.text = Convert.ToString(GameManager.Instance.GetAsteriy().Result);
        bearsAmountText.text = Convert.ToString(GameManager.Instance.bears.Count) + "/" + Convert.ToString(GameManager.Instance.maxBearsAmount);
    }

    public void UpdateBarsStatuses()
    {
        icon.sprite = icons[(int)GameManager.Instance.season];
        seasonHeader.text = SeasonToHeaderText(GameManager.Instance.season);
        //seasonTimeLeft.text = "Ещё " + Convert.ToString((int)GameManager.Instance.GetSeasonTimeLeft()) + " с.";
        seasonDiscription.text = SeasonToDiscriptionText(GameManager.Instance.season);
    }

    private IEnumerator TemperatureChanger()
    {
        while (true)
        {
            temperatureSlider.value = (GameManager.Instance.GetTemperature() + 25f) / 45f;
            temperatureDynamic.GetComponent<TextMeshProUGUI>().text = Convert.ToString((int)GameManager.Instance.GetTemperature()) + " °C";
            yield return new WaitForSeconds(0.1f);
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
            timeLeft.text = "Время до конца смены: " + Convert.ToString((int)GameManager.Instance.GetTimeLeft()) + " с.";
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
                return $"Снижение выработки энергомеда на {15 + 3 * GameManager.Instance.cycleNumber}%";
            case GameManager.Season.Freeze:
                return $"Увеличение потребления энергомеда на {10 + 5 * GameManager.Instance.cycleNumber}%\nУвеличение скорости падения температуры на 25%";
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
                int n1 = 0, n2 = 0, n3 = 0;
                foreach (GameObject room in GameManager.Instance.allRooms)
                {
                    if (room.TryGetComponent<RoomScript>(out RoomScript roomScript))
                    {
                        switch (roomScript.level)
                        {
                            case 1:
                                n1++;
                                break;
                            case 2:
                                n2++;
                                break;
                            case 3:
                                n3++;
                                break;
                        }
                    }
                }
                float honeyToEat = (float)(5 + n1 + 1.05 * n2 + 1.1 * n3);
                if (GameManager.Instance.season == GameManager.Season.Freeze)
                {
                    honeyToEat *= 1f + 0.1f + 0.05f * GameManager.Instance.cycleNumber;
                }
                honeyReducePanel.SetActive(true);
                honeyReduceDynamic.GetComponent<TextMeshProUGUI>().text = Convert.ToString((int)honeyToEat) + "  в минуту";
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
        }
    }
}
