
using System.Collections;
using UnityEngine;

public class ShakeUI : MonoBehaviour
{
	[SerializeField] private float strength;
	[SerializeField] private Transform toShake;
	[SerializeField] private float yPos;
	private Vector3 startPosition;
	private Coroutine shakeCoroutine;
	private int queue = 0;
	private Animator animator;

	private void Start()
	{
		startPosition = toShake.localPosition;
		animator = toShake.GetComponent<Animator>();
	}

	public void Shake()
	{
		animator.SetTrigger("Shake");
	}

	//private IEnumerator ShakeCoroutine()
	//{
	//	while (toShake.localPosition.y < yPos)
	//	{
	//		toShake.Translate(Vector3.up * 1 * Time.deltaTime, Space.Self);
	//		yield return null;
	//	}
	//	while (toShake.localPosition.y > startPosition.y)
	//	{
	//		toShake.Translate(Vector3.down * 1 * Time.deltaTime,Space.Self);
	//		yield return null;
	//	}
	//	toShake.localPosition = startPosition;

	//	if (queue > 1)
	//	{
	//		queue--;
	//		shakeCoroutine = StartCoroutine(ShakeCoroutine());
	//	}
	//	shakeCoroutine = null;
	//}
}
