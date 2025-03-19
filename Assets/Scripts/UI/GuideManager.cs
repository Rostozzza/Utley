using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> slides;
    [SerializeField] private GuideTypes guideType;
    [SerializeField] private bool isGuideOpen = false;

    public void SetGuideType(GuideTypes type) => guideType = type;

    public void ShowGuideByButton()
    {
        if (GetIsGuideOpen()) return;
        SetIsGuideOpen(true);
        ShowGuide(guideType);
    }

    public void ShowGuide(GuideTypes type)
    {
        MenuManager.Instance.SetTablet(true);
        slides[(int)type].SetActive(true);
        Invoke(nameof(StopTime), MenuManager.Instance.tabletAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void HideGuide()
    {
        MenuManager.Instance.SetTablet(false);
        slides.ForEach(slide => slide.SetActive(false));
        gameObject.SetActive(false);
        Time.timeScale = 1;
        SetIsGuideOpen(false);
    }

    private void StopTime() => Time.timeScale = 0;
    private void SetIsGuideOpen(bool set) => isGuideOpen = set;
    private bool GetIsGuideOpen() => isGuideOpen;

    public enum GuideTypes
    {
        Energohoney
    }
}
