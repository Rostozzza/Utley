
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
		while (Vector2.Distance(transform.position, element.position) > 0.1f)
		{
			transform.position = Vector2.Lerp(transform.position,element.position,Time.deltaTime * speed);
			yield return null;
		}
		Destroy(gameObject);
	}
}
