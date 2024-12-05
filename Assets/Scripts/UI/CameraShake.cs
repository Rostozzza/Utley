
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraShake : MonoBehaviour
{
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
	}

	private IEnumerator ImpactCoroutine()
	{
		volume.profile.TryGet(out ChromaticAberration abberation);
		var timer = length;
		Vector3 startPos = transform.position;
		abberation.intensity.value = abberationIntensity;
		while (timer > 0)
		{
			timer -= Time.deltaTime;
			transform.position = transform.position + Random.insideUnitSphere * force;
			//transform.position = startPos + Random.insideUnitSphere * force;
			yield return null;
		}
		timer = abberationLength;
		var abberationDrain = (1f - abberationLength) / abberationLength;
		while (timer > 0)
		{
			timer -= Time.deltaTime;
			abberation.intensity.value -= abberationDrain;
		}
		//transform.position = startPos;
	}
}