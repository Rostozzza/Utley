
using System.Collections;
using UnityEngine;

public class WorkResultIntoUI : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private float acceleration = 100;
	public void FlyTorwardsUI(Transform element)
	{
		StartCoroutine(Fly(element));
	}

	private IEnumerator Fly(Transform element)
	{
		yield return new WaitForSeconds(0.4f);
		var anchoredPos = GetComponent<RectTransform>();
		Vector2 targetedPos;
		targetedPos = Camera.main.WorldToScreenPoint(element.position);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.Instance.GetComponentInChildren<Canvas>().GetComponent<RectTransform>(), (Vector2)targetedPos, Camera.main, out targetedPos);
		Debug.Log(targetedPos);
		float currentSpeed = 1;
		while (Vector2.Distance(anchoredPos.anchoredPosition, targetedPos) > 0.1f)
		{
			anchoredPos.anchoredPosition += ((targetedPos - anchoredPos.anchoredPosition).normalized*Time.deltaTime * currentSpeed);
			currentSpeed += currentSpeed >= speed ? 0 : Time.deltaTime*acceleration;
			yield return null;
		}
		element.GetComponent<ShakeUI>().Shake();
		Destroy(gameObject);
	}
}
