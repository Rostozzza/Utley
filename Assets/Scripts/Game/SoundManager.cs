
using UnityEngine;

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
	[Header("Environment")]
	public AudioClip impactSound;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}