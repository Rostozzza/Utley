
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NotificationTypes;

public class Notification : MonoBehaviour
{
	[SerializeField] private Image background;
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI mainText;
	private Animator animator;

	/// <summary>
	/// Initializes notification. To set up new notification, use presets presented in "NotificationTypes" class.
	/// </summary>
	/// <param name="notification"></param>
	/// <param name="text"></param>
	public void InitializeNotification(NotificationType notification, string text)
	{
		animator = GetComponent<Animator>();
		titleText.text = notification.titleText;
		mainText.text = text;
		background.color = notification.bgColor;
		StartCoroutine(NotificationLifespan(notification.screenTime));
	}

	private IEnumerator NotificationLifespan(float lifespan)
	{
		yield return new WaitForSeconds(lifespan);
		animator.SetTrigger("Fade");
		Destroy(gameObject, 1f);
	}
}
