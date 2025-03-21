using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> slides;
    [SerializeField] private GuideTypes guideType;
    [SerializeField] private bool isGuideOpen = false;
    [SerializeField] private Dictionary<LineRenderer, bool> linesStates = new();

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
        SetHideLinesVFX(true);
    }

    public void HideGuide()
    {
        MenuManager.Instance.SetTablet(false);
        slides.ForEach(slide => slide.SetActive(false));
        gameObject.SetActive(false);
        Time.timeScale = 1;
        SetIsGuideOpen(false);
        Invoke(nameof(ShowLinesDelayed), MenuManager.Instance.tabletAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    private void StopTime() => Time.timeScale = 0;
    private void ShowLinesDelayed() => SetHideLinesVFX(false);
    private void SetIsGuideOpen(bool set) => isGuideOpen = set;
    private bool GetIsGuideOpen() => isGuideOpen;

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