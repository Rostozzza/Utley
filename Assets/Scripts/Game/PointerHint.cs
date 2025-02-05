
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerHint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HintType hintType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hintType != HintType.DontScroll) GameManager.Instance.uiResourceShower.ShowHint(hintType);
        else 
        {
            Camera.main.GetComponent<CameraController>().SetScroll(false);
            GameManager.Instance.SetIsCursorAtUIDontScroll(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hintType != HintType.DontScroll) GameManager.Instance.uiResourceShower.HideHint(hintType);
        else 
        {
            Camera.main.GetComponent<CameraController>().SetScroll(true);
            GameManager.Instance.SetIsCursorAtUIDontScroll(false);
        }
    }

    public enum HintType
    {
        Energohoney,
        Temperature,
        Asterium,
        Bear,
        DontScroll, // xD it's not hint, it's says that if mouse point on it, then mouse scroll shouldn't work
        Ursowaks,
        Prototype,
        Astroluminite,
        Season,
        SeasonDebuff,
        TimeLeft
    } 
}
