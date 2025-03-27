using System.Collections.Generic;
using UnityEngine;
public class RandomTile : MonoBehaviour
{
	[SerializeField] private List<Sprite> sprites;
	void Awake()
	{
		if (MenuManager.Instance.isModded)
		{
			if (MenuManager.Instance.GetComponent<ModManager>().earthSprites.Count > 0)
			{
				sprites = MenuManager.Instance.GetComponent<ModManager>().earthSprites;
			}
		}
		GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Count)];
	}
}