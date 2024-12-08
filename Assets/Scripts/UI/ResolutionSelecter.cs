
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ResolutionSelecter : MonoBehaviour
{
	[SerializeField] private TMP_Dropdown resolutionDropdown;
	private List<Resolution> resolutions;
	private float currentRefreshRate;
	private int currentResolutionIndex = 0;

	public void Start()
	{
		resolutions = Screen.resolutions.ToList();
		resolutionDropdown.ClearOptions();
		List<string> options = resolutions.Select(x => x.width + "x" + x.height + " " + (int)x.refreshRateRatio.value + "Гц").ToList();
		
		resolutionDropdown.AddOptions(options);
		if (!PlayerPrefs.HasKey("SavedResolution"))
		{
			currentResolutionIndex = resolutions.IndexOf(Screen.currentResolution);
		}
		else
		{
			currentResolutionIndex = PlayerPrefs.GetInt("SavedResolution");
		}
		resolutionDropdown.value = currentResolutionIndex;
		resolutionDropdown.RefreshShownValue();
		resolutionDropdown.interactable = true;
	}

	public void SetResolution(int index)
	{
		Resolution resolution = resolutions[index];
		Screen.SetResolution(resolution.width, resolution.height, true);
		PlayerPrefs.SetInt("SavedResolution",index);
		Camera.main.GetComponent<CameraController>().InitPixels();
	}
}
