
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance;
	[Header("Room sounds")]
	public AudioClip asteriumWorkSound;
	public AudioClip cosmodromeWorkSound;
	public AudioClip livingRoomWorkSound;
	public AudioClip researchRoomWorkSound;
	public AudioClip supplyRoomWorkSound;
	public AudioClip energohoneyRoomWorkSound;
	public AudioClip builderRoomWorkSound;
	public AudioClip aiRoomWorkSound;
	[Header("Environment")]
	public AudioClip impactSound;
	[Header("Mono")]
	public AudioClip clickSound;
	public AudioClip bearSelectSound;
	[Header("General Sounds Settings")]
	[SerializeField] private List<AudioMixerGroup> audioMixerGroups;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void PlaySoundOnce(AudioClip audio, MixerGroup output)
	{
		var audioMixerGroup = audioMixerGroups[(int)output];

		if (audio == null || audioMixerGroup == null)
		{
			Debug.Log("<color=red>НЕ СМОГЛИ СЫГРАТЬ ЗВУК</color>");
			return;
		}

		var audioSource = Camera.main.gameObject.AddComponent<AudioSource>();
		audioSource.clip = audio;
		audioSource.loop = false;
		audioSource.outputAudioMixerGroup = audioMixerGroup;
		audioSource.Play();

		Destroy(audioSource, audio.length);
	}

	public enum MixerGroup
	{
		Master,
		SFX,
		Music
	}
}