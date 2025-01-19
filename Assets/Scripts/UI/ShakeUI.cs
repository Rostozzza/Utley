
using System.Collections;
using UnityEngine;

public class ShakeUI : MonoBehaviour
{
	[SerializeField] private float strength;
	[SerializeField] private Transform toShake;
	[SerializeField] private float yPos;
	private float startPosition;
	private Coroutine shakeCoroutine;

	private void Start()
	{
		startPosition = toShake.position.y;
	}

	public void Shake()
	{
		if (shakeCoroutine != null)
		{
			shakeCoroutine = null;
		}
		shakeCoroutine = StartCoroutine(ShakeCoroutine());
	}

	private IEnumerator ShakeCoroutine()
	{
		while (toShake.position.y < yPos)
		{
			toShake.Translate(Vector3.up * strength * Time.deltaTime);
			yield return null;
		}
		while (toShake.position.y > startPosition)
		{
			toShake.Translate(Vector3.down * strength * Time.deltaTime);
			yield return null;
		}
		toShake.position = new Vector3(toShake.position.x,startPosition,toShake.position.z);
	}
}
