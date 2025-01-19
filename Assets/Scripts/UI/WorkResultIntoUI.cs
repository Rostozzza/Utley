
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
		targetedPos = element.transform.position;
		Debug.Log(targetedPos);
		while (Vector2.Distance(anchoredPos.anchoredPosition, targetedPos) > 0.1f)
		{
			anchoredPos.anchoredPosition = Vector2.Lerp(anchoredPos.anchoredPosition, targetedPos, Time.deltaTime * speed);
			yield return null;
		}
		Destroy(gameObject);
	}
}
