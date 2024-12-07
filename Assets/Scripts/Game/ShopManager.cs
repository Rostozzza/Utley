
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
	public static ShopManager Instance;
	public Shop model;
	public string shopName;
	public int bears;
	public int honey;
	public int time;
	public int temperatureBoost;
	public bool isAPIActive;
	[SerializeField] private GameObject buyGrid;
	[SerializeField] private Animator animator;
	[SerializeField] private GameObject shopLoadingScreen;
	[SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
	JsonManager JsonManager;

	//[SerializeField]private List<GameObject> buyItems;
	//[SerializeField] private GameObject itemPrefab;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void OpenShop()
	{
		animator.SetTrigger("OpenShop");
		isAPIActive = MenuManager.Instance.isAPIActive;
		JsonManager = GameManager.Instance.JsonManager;
		if (MenuManager.Instance.isAPIActive)
		{
			shopLoadingScreen.SetActive(true);
			StartCoroutine(LoadShop());
		}

	}

	private IEnumerator LoadShop()
	{
		yield return GameManager.Instance.JsonManager.GetShopModel("HNYShop");
		shopName = model.name;
		bears = model.resources["bears"];
		honey = model.resources["honey"];
		time = model.resources["time"];
		temperatureBoost = model.resources["temperatureBoost"];
		shopLoadingScreen.SetActive(false);
		if (model == null)
		{
			yield break;
		}
		for (int i = 0; i < shopItems.Count; i++)
		{
			shopItems[i].quantity = model.resources[shopItems[i].name];
			shopItems[i].UpdateFields();
		}
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
	public async Task ChangeShopBears(int amount, Log log)
	{
		if (isAPIActive)
		{
			int serverBears = await GetShopHoney();
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
			int serverTime = await GetShopHoney();
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
			int serverTemperatureBoost = await GetShopHoney();
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
}