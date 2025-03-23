using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonToBuild : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject buildPanelPrefab;
    [SerializeField] private RoomType roomType;
    [SerializeField] private Image icon;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private GameObject createdPanel;

    private void Awake()
    {
        icon.sprite = sprites[(int)roomType];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        createdPanel = GetComponentInParent<ChoicesFormer>().ShowRoomPanel(roomType, transform);//GetComponentInParent<ChoicesFormer>().transform.parent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(createdPanel);
    }

    public void BuildByButton()
    {
        GetComponentInParent<ChoicesFormer>().BuildChosenRoom(roomType);
        if (createdPanel != null) Destroy(createdPanel);
    }
}
