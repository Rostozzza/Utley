
using System.Collections;
using UnityEngine;

public class WorkResultIntoUI : MonoBehaviour
{
	[SerializeField] private float speed;
	public void FlyTorwardsUI(Transform element)
	{
		StartCoroutine(Fly(element));
	}

	private IEnumerator Fly(Transform element)
	{
		var anchoredPos = GetComponent<RectTransform>();
		Vector2 targetedPos;
		targetedPos = Camera.main.WorldToScreenPoint(element.position);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.Instance.GetComponentInChildren<Canvas>().GetComponent<RectTransform>(), (Vector2)targetedPos, Camera.main, out targetedPos);
		Debug.Log(targetedPos);
		while (Vector2.Distance(anchoredPos.anchoredPosition, targetedPos) > 0.1f)
		{
			anchoredPos.anchoredPosition += ((targetedPos - anchoredPos.anchoredPosition).normalized*Time.deltaTime * speed);
			yield return null;
		}
		Destroy(gameObject);
	}
}
