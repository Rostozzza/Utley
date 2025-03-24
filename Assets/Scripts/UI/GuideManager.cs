using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GuideManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> slides;
    [SerializeField] private GuideTypes guideType;
    [SerializeField] private bool isGuideOpen = false;
    [SerializeField] private Dictionary<LineRenderer, bool> linesStates = new();
    [SerializeField] private bool wasTabletOpenBeforeGuide = false;

    public void SetGuideType(GuideTypes type) => guideType = type;

    public void ShowGuideByButton()
    {
        if (GetIsGuideOpen()) return;
        SetWasTabletOpen(MenuManager.Instance.tabletAnimator.GetCurrentAnimatorStateInfo(0).IsName("TabletShow"));
        SetIsGuideOpen(true);
        ShowGuide(guideType);
    }

    public void ShowGuide(GuideTypes type)
    {
        if (!GetWasTabletOpen()) MenuManager.Instance.SetTablet(true);
        slides[(int)type].SetActive(true);
        Invoke(nameof(StopTimeDelayed), MenuManager.Instance.tabletAnimator.GetCurrentAnimatorStateInfo(0).length);
        SetHideLinesVFX(true);
        if (type == GuideTypes.Cosmodrome) MenuManager.Instance.GetComponent<Canvas>().sortingOrder = 10000; // forgive me God;
    }

    public void HideGuide()
    {
        if (!GetWasTabletOpen()) MenuManager.Instance.SetTablet(false);
        slides.ForEach(slide => slide.SetActive(false));
        Time.timeScale = 1;
        SetIsGuideOpen(false);
        Invoke(nameof(ShowLinesDelayed), guideType == GuideTypes.Cosmodrome ? 0 : MenuManager.Instance.tabletAnimator.GetCurrentAnimatorStateInfo(0).length);
        if (guideType == GuideTypes.Cosmodrome) MenuManager.Instance.GetComponent<Canvas>().sortingOrder = 9500;
        gameObject.SetActive(false);
    }

    private void StopTimeDelayed() => Time.timeScale = 0;
    private void ShowLinesDelayed() => SetHideLinesVFX(false);
    private void SetIsGuideOpen(bool set) => isGuideOpen = set;
    private bool GetIsGuideOpen() => isGuideOpen;
    private void SetWasTabletOpen(bool set) => wasTabletOpenBeforeGuide = set;
    private bool GetWasTabletOpen() => wasTabletOpenBeforeGuide;

    public enum GuideTypes
    {
        Energohoney,
        Supply,
        Cosmodrome,
        Research,
        Asterium
    }

    void SetHideLinesVFX(bool set) // switch because it's assumes that it will be only changing.
		{
			if (!set) // from menu to game (shows)
			{
				foreach (LineRenderer line in linesStates.Keys)
				{
					line.enabled = linesStates[line];
				}
			}
			else // from game to menu (hides)
			{
				List<LineRenderer> lines = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None).ToList();
				Dictionary<LineRenderer, bool> newLinesStates = new();
				foreach (var line in lines) newLinesStates.Add(line, line.enabled);
				linesStates = newLinesStates;

				lines.ForEach(x => x.enabled = false);
			}
		}
}