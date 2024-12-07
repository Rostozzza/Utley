using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ShopItem : MonoBehaviour
{
	public int price;
	public int maxQuantity;
	public int quantity;
	public string name;
	public string comment;
	public TextMeshProUGUI quantityField;
	public TextMeshProUGUI priceField;
	public TMP_InputField requestedAmount;

	public void UpdateFields()
	{
		quantityField.text = $"{quantity}/{maxQuantity}";
		if (quantity == 0 || int.Parse(requestedAmount.text) >= quantity)
		{
			quantityField.color = Color.red;
			requestedAmount.gameObject.SetActive(false);
		}
	}

	public void BuyItem()
	{
		BuyItemAsync();
	}

	private async Task BuyItemAsync()
	{
		var shopName = ShopManager.Instance.model.name;
		await GameManager.Instance.ChangeHNY(price, new Log
		{
			comment = $"Player {GameManager.Instance.playerName} bought {requestedAmount.text} of {name} from {shopName} for {price} HNY",
			shop_name = shopName,
			player_name = GameManager.Instance.playerName,
			resources_changed = new Dictionary<string, float> { { "HNY", price } }
		});
		switch (name)
		{
			case "honey":
				await GameManager.Instance.ChangeHoney(int.Parse(requestedAmount.text), new Log
				{
					comment = $"Player {GameManager.Instance.playerName} bought {requestedAmount.text} of {name} from {shopName} for {price} HNY",
					resources_changed = new Dictionary<string, float> { { "player_honey_added", +float.Parse(requestedAmount.text) } },
					player_name = GameManager.Instance.playerName
				});

				await ShopManager.Instance.ChangeShopHoney(-int.Parse(requestedAmount.text), new Log
				{
					comment = $"Player {GameManager.Instance.playerName} bought {requestedAmount.text} of {name} from {shopName} for {price} HNY",
					shop_name = shopName,
					resources_changed = new Dictionary<string, float> { { "shop_honey", -float.Parse(requestedAmount.text) } },
					player_name = GameManager.Instance.playerName,
				});
				break;
			case "time":
				//Decrease time

				await ShopManager.Instance.ChangeShopTime(-int.Parse(requestedAmount.text), new Log
				{
					comment = $"Player {GameManager.Instance.playerName}'s work time decreased by {requestedAmount.text} from {shopName} for {price} HNY.",
					shop_name = shopName,
					resources_changed = new Dictionary<string, float> { { "shop_time", -float.Parse(requestedAmount.text) } },
					player_name = GameManager.Instance.playerName,
				});
				break;
			case "temperatureBoost":
				//increase temperature

				await ShopManager.Instance.ChangeShopTemperatureBoost(-int.Parse(requestedAmount.text), new Log
				{
					comment = $"Player {GameManager.Instance.playerName}'s base temperature was urgently raised to maximum for {price} HNY",
					shop_name = shopName,
					resources_changed = new Dictionary<string, float> { { "shop_tempreratureBoost", -float.Parse(requestedAmount.text) } },
					player_name = GameManager.Instance.playerName,
				});
				break;
			case "bears":
				//add new bear

				await ShopManager.Instance.ChangeShopBears(-int.Parse(requestedAmount.text), new Log
				{
					comment = $"Player {GameManager.Instance.playerName} got a new employee for {price} HNY",
					shop_name = shopName,
					resources_changed = new Dictionary<string, float> { { "shop_bears", -float.Parse(requestedAmount.text) } },
					player_name = GameManager.Instance.playerName,
				});
				break;

		}
	}
}
