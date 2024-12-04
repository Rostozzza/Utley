
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerHint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HintType hintType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hintType != HintType.DontScroll) GameManager.Instance.uiResourceShower.ShowHint(hintType);
        else Camera.main.GetComponent<CameraController>().SetScroll(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hintType != HintType.DontScroll) GameManager.Instance.uiResourceShower.HideHint(hintType);
        else Camera.main.GetComponent<CameraController>().SetScroll(true);
    }

    public enum HintType
    {
        Energohoney,
        Temperature,
        Asterium,
        Bear,
        DontScroll
    } 
}
