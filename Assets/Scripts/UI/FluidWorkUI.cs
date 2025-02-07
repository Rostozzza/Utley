
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FluidWorkUI : RoomWorkUI
{
	protected override IEnumerator WorkProcess(float time, float amountOfUnits, Transform ui)
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
			resultText.text = $"{(int)((timer / time)*100f)}%";
			yield return null;
		}
		while (timer / time > 0f)
		{
			timer -= Time.deltaTime*10f;
			resultImage.GetComponent<Image>().fillAmount = 1f - (timer / time);
			resultText.text = $"{(int)((timer / time) * 100f)}%";
			fluidImage.fillAmount = timer / time;
			yield return null;
		}
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
		animator.SetTrigger("Hide");
	}
}
