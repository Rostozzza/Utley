using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomWorkUI : MonoBehaviour
{
	protected Animator animator;
	protected TextMeshProUGUI resultText;
	[SerializeField] protected Transform resultImage;
	[SerializeField] protected Sprite workUnitSprite;
	[SerializeField] protected GameObject workUnitPrefab;
	[SerializeField] protected GameObject workResultToScreenPrefab;
	[SerializeField] protected Transform correspondingUI;
	[SerializeField] protected Sprite workResultSprite;
	protected GridLayoutGroup grid;
	private Vector2 startGridSize;

	public void Start()
	{
		animator = GetComponent<Animator>();
		grid = GetComponentInChildren<GridLayoutGroup>(true);
		resultText = GetComponentInChildren<TextMeshProUGUI>(true);
		startGridSize = grid.cellSize;
	}

	public void StartWork(float time, float amountOfUnits, Transform ui, ResourceType resourceType = ResourceType.None)
	{
		StartCoroutine(WorkProcess(time, amountOfUnits, ui, resourceType));
	}

	public void SetWorkUnitSprite(Sprite sprite)
	{
		workUnitSprite = sprite;
	}

	public void SetResultImage(Sprite image)
	{
		resultImage.GetComponent<Image>().sprite = image;
		workResultSprite = image;
	}

	protected virtual IEnumerator WorkProcess(float time, float amountOfUnits, Transform ui, ResourceType resourceType)
	{
		animator.SetTrigger("Show");
		correspondingUI = ui;
		//grid.cellSize.Set(grid.cellSize.x, 0.7538002f - (0.7538002f / amountOfUnits));
		float timeInterval = time / amountOfUnits;
		int workResults = 0;

		switch (resourceType)
		{
			case ResourceType.Energohoney:
			grid.cellSize = new Vector2(startGridSize.x, startGridSize.y / (amountOfUnits / 5)); // autoresize if energohoney amount != 5 (5 because at default it was 5);
			break;
			
			default:
			break;
		}

		for (int i = 0; i < amountOfUnits; i++)
		{
			var unit = Instantiate(workUnitPrefab, grid.transform);
			unit.GetComponent<Image>().sprite = workUnitSprite;
			workResults++;
			resultText.text = $"+{workResults}";
			yield return new WaitForSeconds(timeInterval);
		}
		animator.SetTrigger("Hide");
		timeInterval = 1f / amountOfUnits;
		for (int i = (int)amountOfUnits; i > 0; i--)
		{
			yield return new WaitForSeconds(timeInterval);
			Destroy(grid.transform.GetChild(0).gameObject);
			var resultInstance = Instantiate(workResultToScreenPrefab, GameManager.Instance.GetComponentInChildren<Canvas>().transform);

			Vector2 ancoredPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.Instance.GetComponentInChildren<Canvas>().GetComponent<RectTransform>(), (Vector2)Camera.main.WorldToScreenPoint(resultImage.transform.position), Camera.main, out ancoredPos);
			Debug.Log(ancoredPos);
			resultInstance.GetComponent<RectTransform>().anchoredPosition = ancoredPos + Random.insideUnitCircle*15f;
			//resultInstance.GetComponent<Image>().sprite = resultImage.GetComponent<Image>().sprite;
			resultInstance.GetComponent<WorkResultIntoUI>().FlyTorwardsUI(correspondingUI,workResultSprite);
			workResults--;
			resultText.text = $"+{workResults}";
		}
		
	}

	public enum ResourceType
	{
		None,
		Energohoney,
	}
}

