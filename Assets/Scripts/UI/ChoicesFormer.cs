using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChoicesFormer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<RoomType> roomsToCreate;
    [Tooltip("Order should match with RoomType except Cosmodrome (leave empty)")][SerializeField] private List<GameObject> roomPrefabs;
    [Tooltip("Order should match with RoomType except Cosmodrome (leave empty)")][SerializeField] private List<Sprite> roomIcons;
    //void Awake()
    //{
    //    foreach (var room in roomsToCreate)
    //    {
    //        GameObject createdRoom = Instantiate(prefab, transform);
    //        FillPrefab(room, createdRoom.GetComponent<ChoiceController>());
    //    }
    //}

    public GameObject ShowRoomPanel(RoomType room, Transform parent)
    {
        GameObject createdRoom = Instantiate(prefab, parent);
        FillPrefab(room, createdRoom.GetComponent<ChoiceController>());
        return createdRoom;
    }

    public void BuildChosenRoom(RoomType roomType)
    {
		EventManager.onRoomQueuedForBuild.Invoke(roomType);
		GameManager.Instance.SelectAndBuild(roomPrefabs[(int)roomType]);
    }

    private void FillPrefab(RoomType room, ChoiceController prefabScript)
    {
        prefabScript.SetMaster(this);
        prefabScript.roomType = room;
        prefabScript.title.text = RoomToText(room, KindOfText.title);
        prefabScript.description.text = RoomToText(room, KindOfText.description);

        Sprite sprite = roomIcons[(int)room];
        if (sprite == null)
        prefabScript.icon.enabled = false;
        else 
        prefabScript.icon.sprite = sprite;
    }

    private string RoomToText(RoomType room, KindOfText kind)
    {
        return kind switch
        {
            KindOfText.title => room switch
            {
                RoomType.Elevator => "Лестница",
                RoomType.Asterium => "Комплекс переработки астерия",
                RoomType.Bed => "Жилой комплекс",
                RoomType.Energohoney => "Комплекс выработки энергомеда",
                RoomType.Research => "Комплекс исследований",
                RoomType.Supply => "Комплекс снабжения",
                RoomType.Build => "Комплекс строительства",
                _ => "Неизвестная комната",
            },

            KindOfText.description => room switch
            {
                RoomType.Elevator => "Используется для перемещения между этажами.",
                RoomType.Asterium => "Перерабатывает необработанный <color=yellow>астерий</color> с космодрома <color=yellow>автоматически</color>. Улучшение ускоряет переработку.",
                RoomType.Bed => "<color=yellow>Увеличивает лимит персонала базы</color>. Воодушевляет <color=yellow>3</color> случайных медведей, повышая их эффективность на <color=yellow>10%</color>. Улучшение ускоряет взаимодействие.",
                RoomType.Energohoney => "Производит <color=yellow>энергомед</color>. Улучшение ускоряет выработку.",
                RoomType.Research => "Создает <color=yellow>опытные образцы</color> и <color=yellow>урсовокс</color>. Улучшение ускоряет производство.",
                RoomType.Supply => "Снабжает комплексы энергией. Настраивает подачу энергии с помощью графов. <color=yellow>Питает комплексы вокруг</color>.",
                RoomType.Build => "Позволяет <color=yellow>строить</color>, <color=yellow>ремонтировать</color> и <color=yellow>улучшать</color> комплексы. Неразрушаемый. Улучшение ускоряет работу.",
                _ => "Неизвестная комната",
            },

            _ => "Неизвестное текстовое поле",
        };
    }

    private enum KindOfText
    {
        title,
        description,
        cost
    }
}
