using System;
using TMPro;
using Unity.VisualScripting;
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

    private void Start()
    {
        UpdateIndicators();
        honeyReducePanel.SetActive(false);
        temperaturePanel.SetActive(false);
        asteriumPanel.SetActive(false);
        bearPanel.SetActive(false);
    }

    /// <summary>
    /// Updates UI indicators (honey, asteriy, etc.)
    /// </summary>
    public void UpdateIndicators()
    {
        energoHoneyAmountText.text = Convert.ToString(GameManager.Instance.GetHoney().Result);
        asteriyAmountText.text = Convert.ToString(GameManager.Instance.GetAsteriy().Result);
        bearsAmountText.text = Convert.ToString(GameManager.Instance.bears.Count) + "/" + Convert.ToString(GameManager.Instance.maxBearsAmount);
        temperatureSlider.value = 1f; // ???
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
                int honeyToEat = (int)(5 + n1 + 1.1 * n2 + 1.2 * n3);
                honeyReducePanel.SetActive(true);
                honeyReduceDynamic.GetComponent<TextMeshProUGUI>().text = Convert.ToString(honeyToEat);
                break;

            case PointerHint.HintType.Temperature:
                temperaturePanel.SetActive(true);
                temperatureDynamic.GetComponent<TextMeshProUGUI>().text = Convert.ToString(15) + " °C";
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
