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

	public void Start()
	{
		animator = GetComponent<Animator>();
		grid = GetComponentInChildren<GridLayoutGroup>(true);
		resultText = GetComponentInChildren<TextMeshProUGUI>(true);
	}

	public void StartWork(float time, float amountOfUnits, Transform ui)
	{
		StartCoroutine(WorkProcess(time, amountOfUnits, ui));
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

	protected virtual IEnumerator WorkProcess(float time, float amountOfUnits, Transform ui)
	{
		animator.SetTrigger("Show");
		correspondingUI = ui;
		//grid.cellSize.Set(grid.cellSize.x, 0.7538002f - (0.7538002f / amountOfUnits));
		float timeInterval = time / amountOfUnits;
		int workResults = 0;
		for (int i = 0; i < amountOfUnits; i++)
		{
			yield return new WaitForSeconds(timeInterval);
			Instantiate(workUnitPrefab, grid.transform).GetComponent<Image>().sprite = workUnitSprite;
			workResults++;
			resultText.text = $"+{workResults}";
		}
		timeInterval = 1f / amountOfUnits;
		for (int i = 0; i < amountOfUnits; i++)
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
		animator.SetTrigger("Hide");
	}
}

