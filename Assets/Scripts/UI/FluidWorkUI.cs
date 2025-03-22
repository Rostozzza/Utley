
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FluidWorkUI : RoomWorkUI
{
	protected override IEnumerator WorkProcess(float time, float amountOfUnits, Transform ui, ResourceType resourceType = ResourceType.None)
	{
		animator.SetTrigger("Show");
		correspondingUI = ui;
		float timer = 0f;
		resultImage.GetComponent<Image>().fillAmount = 0;
		int workResults = 0;

		var fluidImage = grid.GetComponent<Image>();
		while (timer / time < 1f)
		{
			timer += Time.deltaTime;
			fluidImage.fillAmount = timer / time;
			resultImage.GetComponent<Image>().fillAmount = (timer / time);
			resultText.text = $"{(int)((timer / time) * 100f)}%";
			yield return null;
		}
		timer = 1f;
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			resultText.text = $"{(int)((timer / 1f) * 100f)}%";
			fluidImage.fillAmount = timer / 1f;
			yield return null;
		}
		animator.SetTrigger("Hide");
		resultImage.GetComponent<Image>().fillAmount = 0;
		try
		{
			var resultInstance = Instantiate(workResultToScreenPrefab, GameManager.Instance.GetComponentInChildren<Canvas>().transform);

			Vector2 ancoredPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.Instance.GetComponentInChildren<Canvas>().GetComponent<RectTransform>(), (Vector2)Camera.main.WorldToScreenPoint(resultImage.transform.position), Camera.main, out ancoredPos);
			Debug.Log(ancoredPos);
			resultInstance.GetComponent<RectTransform>().anchoredPosition = ancoredPos + Random.insideUnitCircle * 15f;
			//resultInstance.GetComponent<Image>().sprite = resultImage.GetComponent<Image>().sprite;
			resultInstance.GetComponent<WorkResultIntoUI>().FlyTorwardsUI(correspondingUI, workResultSprite);
		}
		catch { }
		resultText.text = "";
		resultImage.GetComponent<Image>().fillAmount = 0;
		
	}
}
