using UnityEngine;
using UnityEngine.UI;

public class SpriteFiller : MonoBehaviour
{
	[SerializeField] private Image fieldToFill;
	public string _name;

	private void Awake()
	{
		MenuManager.Instance.gameObject.GetComponentInChildren<ModManager>().AddSpriteFiller(this);
	}

	public void FillSprite(byte[] bytes)
	{
		var texture = new Texture2D(2,2);
		texture.LoadImage(bytes);
		fieldToFill.sprite = Sprite.Create(texture,fieldToFill.sprite.rect,fieldToFill.sprite.pivot);
	}
	public void FillSprite(Sprite sprite)
	{
		fieldToFill.sprite = sprite;
	}
}
