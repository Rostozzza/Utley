
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraShake : MonoBehaviour
{
	[Header("AudioSettings")]
	[SerializeField] private float xRange;
	[SerializeField] private Transform soundTransform;
	[SerializeField] private AudioSource audioSource;
	[Header("Shake settings")]
	[SerializeField] private float force;
	[SerializeField] private float length;
	[Header("Abberation settings")]
	[SerializeField] private Volume volume;
	[SerializeField] private float abberationLength;
	[SerializeField] private float abberationIntensity;

	public void MeteorImpact()
	{
		StartCoroutine(ImpactCoroutine());
		StartCoroutine(AbberationDrain());
	}

	private IEnumerator ImpactCoroutine()
	{
		soundTransform.Translate(transform.position.x + Random.Range(-xRange,xRange),0,0);
		audioSource.PlayOneShot(SoundManager.Instance.impactSound);
		var timer = length;
		Vector3 startPos = transform.position;
		while (timer > 0)
		{
			timer -= Time.deltaTime;
			transform.position = transform.position + (Vector3)Random.insideUnitCircle * force;
			//transform.position = startPos + Random.insideUnitSphere * force;
			yield return null;
		}
		//transform.position = startPos;
	}
	private IEnumerator AbberationDrain()
	{
		volume.profile.TryGet(out ChromaticAberration abberation);
		abberation.intensity.value = abberationIntensity;
		var timer = abberationLength;
		var abberationDrain = (abberationIntensity) / abberationLength;
		while (timer > 0)
		{
			timer -= Time.deltaTime;
			abberation.intensity.Override(abberation.intensity.value - abberationDrain * Time.deltaTime);
			yield return null;
		}
		abberation.intensity.value = 0;
	}
}