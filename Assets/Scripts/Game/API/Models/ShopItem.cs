using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ShopItem : MonoBehaviour
{
	public float price;
	public int maxQuantity;
	public int quantity;
	public string name;
	public string comment;
	public TextMeshProUGUI quantityField;
	public TextMeshProUGUI priceField;
	public TMP_InputField requestedAmount;
	public TextMeshProUGUI costOutput;

	public void UpdateFields()
	{
		if (price > 0)
		{
			costOutput.text = "+ " + (price * (requestedAmount.text.Length > 0 ? int.Parse(requestedAmount.text) : 0)).ToString() + " М.Е.Д.";
			return;
		}
		if (requestedAmount.text.Length > 0 && int.Parse(requestedAmount.text) <= maxQuantity)
		{
			quantityField.text = $"{Mathf.Clamp(quantity - int.Parse(requestedAmount.text),0,99999999)}/{maxQuantity}";
		}
		else
		{
			quantityField.text = $"{Mathf.Clamp(quantity, 0, 9999)}/{maxQuantity}";
		}
		if (quantity == 0 || (requestedAmount.text.Length > 0 && int.Parse(requestedAmount.text) >= quantity))
		{
			quantityField.color = Color.red;
		}
		else
		{
			quantityField.color = Color.black;
		}
		costOutput.text = (price * (requestedAmount.text.Length > 0 ? int.Parse(requestedAmount.text) : 0)).ToString() + " М.Е.Д.";
	}

	public async Task BuyItemAsync()
	{
		var shopName = ShopManager.Instance.shopName;
		if (price < 0)
		{
			switch (name)
			{
				case "bears":
					if (await ShopManager.Instance.GetShopBears() <= 0)
					{
						Debug.Log("Cant buy! No more bears");
						requestedAmount.text = "";
						return;
					}
					break;
				case "honey":
					if (await ShopManager.Instance.GetShopHoney() <= 0)
					{
						Debug.Log("Cant buy! No more honey");
						requestedAmount.text = "";
						return;
					}
					break;
				case "time":
					if (await ShopManager.Instance.GetShopTime() <= 0)
					{
						Debug.Log("Cant buy! No more time in shop");
						requestedAmount.text = "";
						return;
					}
					break;
				case "temperatureBoost":
					if (await ShopManager.Instance.GetShopTemperatureBoost() <= 0)
					{
						Debug.Log("Cant buy! No more boosters");
						requestedAmount.text = "";
						return;
					}
					break;
			}
			if (requestedAmount.text.Length <= 0)
			{
				requestedAmount.text = "";
				return;
			}
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
					GameManager.Instance.DecreaseTime();
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
					GameManager.Instance.BoostTemperature();
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
					await GameManager.Instance.SpawnNewBear(int.Parse(requestedAmount.text));

					await ShopManager.Instance.ChangeShopBears(-int.Parse(requestedAmount.text), new Log
					{
						comment = $"Player {GameManager.Instance.playerName} got a new employee for {price} HNY",
						shop_name = shopName,
						resources_changed = new Dictionary<string, float> { { "shop_bears", -float.Parse(requestedAmount.text) } },
						player_name = GameManager.Instance.playerName,
					});
					break;
				case "asterium":
					await GameManager.Instance.ChangeAsteriy(int.Parse(requestedAmount.text), new Log
					{
						comment = $"Player {GameManager.Instance.playerName} bought {requestedAmount.text} of {name} from {shopName} for {price} HNY",
						resources_changed = new Dictionary<string, float> { { "player_asterium_added", +float.Parse(requestedAmount.text) } },
						player_name = GameManager.Instance.playerName
					});

					await ShopManager.Instance.ChangeShopAsterium(-int.Parse(requestedAmount.text), new Log
					{
						comment = $"Player {GameManager.Instance.playerName} bought {requestedAmount.text} of {name} from {shopName} for {price} HNY",
						shop_name = shopName,
						resources_changed = new Dictionary<string, float> { { "shop_asterium", -float.Parse(requestedAmount.text) } },
						player_name = GameManager.Instance.playerName,
					});
					break;
			}
			await GameManager.Instance.ChangeHNY(price * int.Parse(requestedAmount.text), new Log
			{
				comment = $"Player {GameManager.Instance.playerName} bought {requestedAmount.text} of {name} from {shopName} for {price} HNY",
				shop_name = shopName,
				player_name = GameManager.Instance.playerName,
				resources_changed = new Dictionary<string, float> { { "HNY", price * int.Parse(requestedAmount.text) } }
			});
			UpdateFields();
		}
		else
		{
			if (requestedAmount.text.Length <= 0)
			{
				Debug.Log("No selling value!");
				return;
			}
			var amountToSell = float.Parse(requestedAmount.text);
			switch (name)
			{
				case "honey":
					if (await GameManager.Instance.GetHoney() < amountToSell)
					{
						Debug.Log("Trying to sell more than exists!");
						UpdateFields();
						return;
					}
					await GameManager.Instance.ChangeHoney(-int.Parse(requestedAmount.text), new Log
					{
						comment = $"Player {GameManager.Instance.playerName} sold {requestedAmount.text} honey to shop {shopName} for {price * int.Parse(requestedAmount.text)} HNY",
						resources_changed = new Dictionary<string, float> { { "player_honey", -float.Parse(requestedAmount.text) } },
						player_name = GameManager.Instance.playerName
					});
					break;
				case "asterium":
					if (await GameManager.Instance.GetAsteriy() < amountToSell)
					{
						Debug.Log("Trying to sell more than exists!");
						UpdateFields();
						return;
					}
					await GameManager.Instance.ChangeAsteriy(-int.Parse(requestedAmount.text), new Log
					{
						comment = $"Player {GameManager.Instance.playerName} sold {requestedAmount.text} asterium to shop {shopName} for {price * int.Parse(requestedAmount.text)} HNY",
						resources_changed = new Dictionary<string, float> { { "player_asterium", -float.Parse(requestedAmount.text) } },
						player_name = GameManager.Instance.playerName
					});
					break;
				case "astroluminite":
					if (await GameManager.Instance.GetAstroluminite() < amountToSell)
					{
						Debug.Log("Trying to sell more than exists!");
						UpdateFields();
						return;
					}
					await GameManager.Instance.ChangeAstroluminite(-int.Parse(requestedAmount.text), new Log
					{
						comment = $"Player {GameManager.Instance.playerName} sold {requestedAmount.text} astroluminite to shop {shopName} for {price * int.Parse(requestedAmount.text)} HNY",
						resources_changed = new Dictionary<string, float> { { "player_astroluminite", -float.Parse(requestedAmount.text) } },
						player_name = GameManager.Instance.playerName
					});
					break;
				case "prototype":
					if (await GameManager.Instance.GetPrototype() < amountToSell)
					{
						Debug.Log("Trying to sell more than exists!");
						UpdateFields();
						return;
					}
					await GameManager.Instance.ChangePrototype(-int.Parse(requestedAmount.text), new Log
					{
						comment = $"Player {GameManager.Instance.playerName} sold {requestedAmount.text} prototypes to shop {shopName} for {price * int.Parse(requestedAmount.text)} HNY",
						resources_changed = new Dictionary<string, float> { { "player_prototype", -float.Parse(requestedAmount.text) } },
						player_name = GameManager.Instance.playerName
					});
					break;
				case "ursowaks":
					if (await GameManager.Instance.GetUrsowaks() < amountToSell)
					{
						Debug.Log("Trying to sell more than exists!");
						UpdateFields();
						return;
					}
					await GameManager.Instance.ChangeUrsowaks(-int.Parse(requestedAmount.text), new Log
					{
						comment = $"Player {GameManager.Instance.playerName} sold {requestedAmount.text} ursowaks to shop {shopName} for {price * int.Parse(requestedAmount.text)} HNY",
						resources_changed = new Dictionary<string, float> { { "player_ursowaks", -float.Parse(requestedAmount.text) } },
						player_name = GameManager.Instance.playerName
					});
					break;
			}
			await GameManager.Instance.ChangeHNY(price * int.Parse(requestedAmount.text), new Log
			{
				comment = $"Player {GameManager.Instance.playerName} sold {requestedAmount.text} of {name} from {shopName} for {price} HNY",
				shop_name = shopName,
				player_name = GameManager.Instance.playerName,
				resources_changed = new Dictionary<string, float> { { "HNY", price * amountToSell } }
			});
			UpdateFields();
		}
		ShopManager.Instance.HNYField.text = Math.Round((await GameManager.Instance.GetHNY()), 2).ToString();
	}
}
