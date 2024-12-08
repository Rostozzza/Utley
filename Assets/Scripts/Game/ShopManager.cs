
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System;

public class ShopManager : MonoBehaviour
{
	public static ShopManager Instance;
	public Shop model;
	public string shopName;
	public int bears;
	public int honey;
	public int time;
	public int temperatureBoost;
	public int asterium;
	public bool isAPIActive;
	public TextMeshProUGUI HNYField;
	[SerializeField] private GameObject buyGrid;
	[SerializeField] private Animator animator;
	[SerializeField] private GameObject shopLoadingScreen;
	[SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
	[SerializeField] private List<ShopItem> shopItemsToSell = new List<ShopItem>();
	JsonManager JsonManager;
	private bool isOpened = false;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void OpenShop()
	{
		if (!isOpened)
		{
			isOpened = true;
			animator.SetTrigger("OpenShop");
			isAPIActive = MenuManager.Instance.isAPIActive;
			JsonManager = GameManager.Instance.JsonManager;
			if (MenuManager.Instance.isAPIActive)
			{
				shopLoadingScreen.SetActive(true);
				StartCoroutine(LoadShop());
				GameManager.Instance.JsonManager.GetShopModel("HNYShop");
				return;
			}
			HNYField.text = Math.Round(GameManager.Instance.HNY, 2).ToString();
		}
		else
		{
			isOpened = false;
			animator.SetTrigger("CloseShop");
			isAPIActive = MenuManager.Instance.isAPIActive;
			return;
		}
	}

	private IEnumerator LoadShop()
	{
		while (model == null)
		{
			yield return null;
		}
		HNYField.text = Math.Round(GameManager.Instance.HNY, 2).ToString();
		shopName = model.name;
		bears = model.resources["bears"];
		honey = model.resources["honey"];
		time = model.resources["time"];
		temperatureBoost = model.resources["temperatureBoost"];
		asterium = model.resources["asterium"];
		shopLoadingScreen.SetActive(false);
		for (int i = 0; i < shopItems.Count; i++)
		{
			shopItems[i].quantity = model.resources[shopItems[i].name];
			shopItems[i].UpdateFields();
		}
	}

	public void BuyAll()
	{
		BuyAllAsync();
	}

	private async Task BuyAllAsync()
	{
		shopLoadingScreen.SetActive(true);
		foreach (var item in shopItems)
		{
			if (item.requestedAmount.text.Length > 0)
			{
				await item.BuyItemAsync();
			}
		}
		shopLoadingScreen.SetActive(false);
	}

	public void SellAll()
	{
		SellAllAsync();
	}

	private async Task SellAllAsync()
	{
		shopLoadingScreen.SetActive(true);
		foreach (var item in shopItemsToSell)
		{
			if (item.requestedAmount.text.Length > 0)
			{
				await item.BuyItemAsync();
			}
		}
		shopLoadingScreen.SetActive(false);
	}

	#region Get/Change
	public async Task<int> GetShopHoney()
	{
		if (isAPIActive)
		{
			var model = await JsonManager.GetShopModel(shopName);
			this.model = model;
			honey = model.resources["honey"];
			return model.resources["honey"];
		}
		return honey;
	}

	public async Task<int> GetShopAsterium()
	{
		if (isAPIActive)
		{
			var model = await JsonManager.GetShopModel(shopName);
			this.model = model;
			asterium = model.resources["asterium"];
			return model.resources["asterium"];
		}
		return asterium;
	}

	public async Task<int> GetShopBears()
	{
		if (isAPIActive)
		{
			var model = await JsonManager.GetShopModel(shopName);
			this.model = model;
			bears = model.resources["bears"];
			return model.resources["bears"];
		}
		return bears;
	}
	public async Task<int> GetShopTime()
	{
		if (isAPIActive)
		{
			var model = await JsonManager.GetShopModel(shopName);
			this.model = model;
			time = model.resources["time"];
			return model.resources["time"];
		}
		return time;
	}
	public async Task<int> GetShopTemperatureBoost()
	{
		if (isAPIActive)
		{
			var model = await JsonManager.GetShopModel(shopName);
			this.model = model;
			temperatureBoost = model.resources["temperatureBoost"];
			return model.resources["temperatureBoost"];
		}
		return temperatureBoost;
	}

	public async Task ChangeShopHoney(int amount, Log log)
	{
		if (isAPIActive)
		{
			int serverHoney = await GetShopHoney();
			serverHoney += amount;
			serverHoney = Mathf.Clamp(serverHoney, 0, 999);
			honey = serverHoney;
			await JsonManager.SaveShopToJson(shopName);
			await JsonManager.CreateLog(log);
			shopItems.First(x => x.name == "honey").quantity = serverHoney;
			shopItems.First(x => x.name == "honey").UpdateFields();
			return;
		}
		shopItems.First(x => x.name == "honey").quantity += amount;
		shopItems.First(x => x.name == "honey").UpdateFields();
		honey += amount;
		honey = Mathf.Clamp(honey, 0, 999);
	}

	public async Task ChangeShopAsterium(int amount, Log log)
	{
		if (isAPIActive)
		{
			int serverAsterium = await GetShopAsterium();
			serverAsterium += amount;
			serverAsterium = Mathf.Clamp(serverAsterium, 0, 999);
			asterium = serverAsterium;
			await JsonManager.SaveShopToJson(shopName);
			await JsonManager.CreateLog(log);
			shopItems.First(x => x.name == "asterium").quantity = serverAsterium;
			shopItems.First(x => x.name == "asterium").UpdateFields();
			return;
		}
		shopItems.First(x => x.name == "asterium").quantity += amount;
		shopItems.First(x => x.name == "asterium").UpdateFields();
		asterium += amount;
		asterium = Mathf.Clamp(honey, 0, 999);
	}

	public async Task ChangeShopBears(int amount, Log log)
	{
		if (isAPIActive)
		{
			int serverBears = await GetShopBears();
			serverBears += amount;
			serverBears = Mathf.Clamp(serverBears, 0, 999);
			bears = serverBears;
			await JsonManager.SaveShopToJson(shopName);
			await JsonManager.CreateLog(log);
			shopItems.First(x => x.name == "bears").quantity = serverBears;
			shopItems.First(x => x.name == "bears").UpdateFields();
			return;
		}
		shopItems.First(x => x.name == "bears").quantity += amount;
		shopItems.First(x => x.name == "bears").UpdateFields();
		bears += amount;
		bears = Mathf.Clamp(bears, 0, 999);
	}
	public async Task ChangeShopTime(int amount, Log log)
	{
		if (isAPIActive)
		{
			int serverTime = await GetShopTime();
			serverTime += amount;
			serverTime = Mathf.Clamp(serverTime, 0, 999);
			time = serverTime;
			await JsonManager.SaveShopToJson(shopName);
			await JsonManager.CreateLog(log);
			shopItems.First(x => x.name == "time").quantity = serverTime;
			shopItems.First(x => x.name == "time").UpdateFields();
			return;
		}
		time += amount;
		shopItems.First(x => x.name == "time").quantity += amount;
		shopItems.First(x => x.name == "time").UpdateFields();
		time = Mathf.Clamp(time, 0, 999);
	}
	public async Task ChangeShopTemperatureBoost(int amount, Log log)
	{
		if (isAPIActive)
		{
			int serverTemperatureBoost = await GetShopTemperatureBoost();
			serverTemperatureBoost += amount;
			serverTemperatureBoost = Mathf.Clamp(serverTemperatureBoost, 0, 999);
			temperatureBoost = serverTemperatureBoost;
			await JsonManager.SaveShopToJson(shopName);
			await JsonManager.CreateLog(log);
			shopItems.First(x => x.name == "temperatureBoost").quantity = serverTemperatureBoost;
			shopItems.First(x => x.name == "temperatureBoost").UpdateFields();
			return;
		}
		temperatureBoost += amount;
		shopItems.First(x => x.name == "temperatureBoost").quantity += amount;
		shopItems.First(x => x.name == "temperatureBoost").UpdateFields();
		temperatureBoost = Mathf.Clamp(temperatureBoost, 0, 999);
	}
	#endregion

	public bool GetIsOpen()
	{
		return isOpened;
	}
}