
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerHint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private HintType hintType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (hintType)
        {
            case HintType.DontScroll:
                Camera.main.GetComponent<CameraController>().SetScroll(false);
                GameManager.Instance.SetIsCursorAtUIDontScroll(true);
                break;
            case HintType.AtUI:
                GameManager.Instance.SetIsCursorAtUI(true);
                break;
            default:
                GameManager.Instance.uiResourceShower.ShowHint(hintType);
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (hintType)
        {
            case HintType.DontScroll:
                Camera.main.GetComponent<CameraController>().SetScroll(true);
                GameManager.Instance.SetIsCursorAtUIDontScroll(false);
                break;
            case HintType.AtUI:
                GameManager.Instance.SetIsCursorAtUI(false); // by some reason, if gameObkect makes inactive, this case doesnt works, so we need to decrement when gameObject makes inactive;
                break;
            default:
                GameManager.Instance.uiResourceShower.HideHint(hintType);
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (hintType)
        {
            case HintType.AtUI:
                Invoke(nameof(RemoveFromCursorAtUI), 0.1f);
                break;
            default:
                break;
        }
    }

    private void RemoveFromCursorAtUI()
    {
        GameManager.Instance.SetIsCursorAtUI(false);
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
        TimeLeft,
        AtUI // same thing as DontScroll, but allows scrolling;
    } 
}
