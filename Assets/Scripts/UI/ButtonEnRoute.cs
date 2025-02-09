
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEnRoute : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private TextMeshProUGUI buttonText;
	[SerializeField] private Image buttonImage;
	private bool isPressed = false;

	public void SetButtonState(bool state)
	{
		GetComponent<Button>().interactable = !state;
		buttonImage.raycastTarget = !state;
		isPressed = state;
	}

	public bool IsButtonPressed()
	{
		return isPressed;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		buttonText.text = "МЕДВЕДЬ В ПУТИ";
		buttonImage.color = new Color(1, 1, 0, 0.75f);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		buttonText.text = "ОТМЕНА";
		buttonImage.color = new Color(1, 0, 0, 0.75f);
	}
}
