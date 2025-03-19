using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> slides;
    [SerializeField] private GuideTypes guideType;

    public void SetGuideType(GuideTypes type) => guideType = type;

    public void ShowGuideByButton()
    {
        ShowGuide(guideType);
    }

    public void ShowGuide(GuideTypes type)
    {
        MenuManager.Instance.SetTablet(true);
        slides[(int)type].SetActive(true);
    }

    public void HideGuide()
    {
        MenuManager.Instance.SetTablet(false);
        slides.ForEach(slide => slide.SetActive(false));
        gameObject.SetActive(false);
    }

    public enum GuideTypes
    {
        Energohoney
    }
}
