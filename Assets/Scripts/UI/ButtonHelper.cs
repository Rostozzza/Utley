using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHelper : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySoundOnce(SoundManager.Instance.clickSound, SoundManager.MixerGroup.SFX);
    }
}
