using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerHint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HintType hintType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.uiResourceShower.ShowHint(hintType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.uiResourceShower.HideHint(hintType);
    }

    public enum HintType
    {
        Energohoney,
        Temperature
    } 
}
