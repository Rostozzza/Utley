
using System.Collections;
using UnityEngine;

public class ShakeUI : MonoBehaviour
{
	[SerializeField] private float strength;
	[SerializeField] private Transform toShake;
	[SerializeField] private float yPos;
	private float startPosition;
	private Coroutine shakeCoroutine;
	private int queue = 0;

	private void Start()
	{
		startPosition = toShake.localPosition.y;
	}

	public void Shake()
	{
		if (shakeCoroutine != null)
		{
			queue++;
			return;
		}
		queue++;
		shakeCoroutine = StartCoroutine(ShakeCoroutine());
	}

	private IEnumerator ShakeCoroutine()
	{
		while (toShake.localPosition.y < yPos)
		{
			toShake.Translate(Vector3.up * 1 * Time.deltaTime, Space.Self);
			yield return null;
		}
		while (toShake.localPosition.y > startPosition)
		{
			toShake.Translate(Vector3.down * 1 * Time.deltaTime,Space.Self);
			yield return null;
		}
		toShake.localPosition = new Vector3(toShake.position.x,startPosition,toShake.position.z);
		if (queue > 0)
		{
			queue--;
			shakeCoroutine = StartCoroutine(ShakeCoroutine());
		}
	}
}
