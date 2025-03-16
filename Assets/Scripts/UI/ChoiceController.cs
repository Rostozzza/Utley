using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceController : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI title;
    [SerializeField] public TextMeshProUGUI description;
    [SerializeField] public TextMeshProUGUI cost;
    [SerializeField] public Image icon;
    [SerializeField] public RoomType roomType;
    private ChoicesFormer master;
    public void SelectBuilding()
    {
        master.BuildChosenRoom(roomType);
    }

    public void SetMaster(ChoicesFormer master) => this.master = master;
}
