using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ModUIManager : MonoBehaviour
{
	public void SetAllButtons(Sprite newSprite)
	{
		FindObjectsByType<ButtonHelper>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(x => x.gameObject).ToList().ForEach(x => x.GetComponent<Image>().sprite = newSprite);
	}
}