using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopHNYPredictionCounter : MonoBehaviour
{
	[SerializeField] private List<ShopItem> itemsToCount;
	[SerializeField] private TextMeshProUGUI counterText;

	public void RecalculateCounter()
	{
		float prediction = 0;
		foreach (ShopItem item in itemsToCount)
		{
			prediction += item.GetCurrentHNYPrediction();
		}
		counterText.text = $"<color={(prediction > 0 ? "green" : "red")}>{(prediction == 0 ? "" : prediction.ToString())} {(prediction != 0 ? "М.Е.Д." : "")}";
	}
}

