
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
	public static ShopManager Instance;

	[SerializeField] private GameObject screen;
	[SerializeField] private Shop model;
	[SerializeField] private Transform grid;
	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private GameObject unitPrefab;
	[SerializeField] private List<Bear> modelsToBuy;
	[SerializeField] private RoomScript cosmodrome;
	[Header("Bear options")]
	[SerializeField] private List<GameObject> rotationButtons;
	[SerializeField] private List<GameObject> shopButtons;
	private int currentRotation;
	[SerializeField] private List<string> availableNames;
	[SerializeField] private List<string> availableFur;
	[SerializeField] private List<string> availableTops;
	[SerializeField] private List<string> availableDowns;

	public void Awake()
	{
		Instance = this;
		GenerateItems();
	}

	public void OpenShop()
	{
		screen.SetActive(true);
	}

	public void CloseShop()
	{
		cosmodrome.InterruptWork();
		screen.SetActive(false);
	}

	public void GenerateRotation()
	{
		currentRotation++;
		if (currentRotation >= rotationButtons.Count)
		{
			currentRotation = 0;
		}
		rotationButtons.ForEach(x => x.SetActive(false));
		rotationButtons[currentRotation].SetActive(true);
	}

	public void GenerateItems()
	{
		for (int i = 0; i < modelsToBuy.Count; i++)
		{
			var newModel = new Bear
			{
				Name = availableNames[Random.Range(0,availableNames.Count)],
				Top = availableTops[Random.Range(0, availableNames.Count)],
				Bottom = availableDowns[Random.Range(0, availableNames.Count)],
				Fur = availableFur[Random.Range(0, availableNames.Count)],
				Level = 0
			};
			modelsToBuy.Add(newModel);
		}
	}

	public async Task BuyItem(int index)
	{
		//cosmodrome.StartCoroutine(cosmodrome.WorkStatus());
		var model = modelsToBuy[index];
		var unit = Instantiate(unitPrefab);
		var unitScript = unit.GetComponent<UnitScript>();
		unitScript.LoadDataFromModel(model);
		GameManager.Instance.bears.Add(unit);
		
		Destroy(shopButtons[index]);
		modelsToBuy.Remove(model);
	}
}