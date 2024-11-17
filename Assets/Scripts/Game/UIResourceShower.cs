using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceShower : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energoHoneyAmountText;
    [SerializeField] private TextMeshProUGUI asteriyAmountText;
    [SerializeField] private TextMeshProUGUI bearsAmountText;
    [SerializeField] private Slider temperatureSlider;

    /// <summary>
    /// Updates UI indicators (honey, asteriy, etc.)
    /// </summary>
    public void UpdateIndicators()
    {
        energoHoneyAmountText.text = Convert.ToString(GameManager.Instance.GetHoney());
        asteriyAmountText.text = Convert.ToString(GameManager.Instance.GetAsteriy());
        bearsAmountText.text = "99/" + Convert.ToString(GameManager.Instance.bears.Count); // "99/" is temporarily - needs to know living rooms amount
        temperatureSlider.value = 1f; // ???
    }
}
