using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using System.Collections;

public class RoomStatsController : MonoBehaviour
{
    [SerializeField] private RoomScript roomScript;
    [SerializeField] private GameObject title;
    [SerializeField] private bool isInfoActive = false;
	[SerializeField] private GameObject infoText;
    [SerializeField] private List<GameObject> toHideWhenInfo;
    [SerializeField] private GameObject generalInfo;
    [SerializeField] private GameObject isEfficUpgraded;
    [SerializeField] private GameObject resourceEmoji;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private bool isStatsActive = false;
    [SerializeField] private float offsetZ;

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        transform.position = transform.position + new Vector3(0, 0, offsetZ);
        if (GetComponentInParent<RoomScript>().resource != RoomScript.Resources.Cosmodrome) rectTransform.localPosition = new Vector3(2, 0, rectTransform.localPosition.z);
        InitStatsScreen();
    }

	public void SetStatsScreenShow(bool set)
	{
        isStatsActive = set;
		if (set)
		{
			roomScript.GetRoomStatsScreen().SetActive(true);
		}
		else
		{
            if (isInfoActive) SwitchInfo();
			roomScript.GetRoomStatsScreen().SetActive(false);
		}
	}

	public void SwitchInfo()
	{
		isInfoActive = !isInfoActive;
        infoText.SetActive(isInfoActive);
        toHideWhenInfo.ForEach(x => x.SetActive(!isInfoActive));
	}

    private void InitStatsScreen()
    {
        title.GetComponent<TextMeshProUGUI>().text = ResourceToRoomTitle(roomScript.resource);
        infoText.GetComponent<TextMeshProUGUI>().text = ResourceToRoomInfo(roomScript.resource);
        generalInfo.GetComponent<TextMeshProUGUI>().text = RoomDataToGeneralInfoText(roomScript);
        RefreshDescription();
    }

    public void RefreshDescription()
    {
        isEfficUpgraded.GetComponent<TextMeshProUGUI>().text = roomScript.GetIsEfficBearWorking() ? "<color=yellow>Эффективность повышена</color>" : "Стандартная эффективность";
    }

    private string RoomDataToGeneralInfoText(RoomScript roomScript)
    {
        string toReturn =
            $"{ResourceToText(roomScript.resource)}.\n" +
            $"Время работы {roomScript.GetInteractionTime()} с.\n" +
            $"Эффективный специалист:\n" +
            $"<color=yellow>{ResourceToEfficBearText(roomScript.resource)}.</color>\n";
        return toReturn;
    }

    private string ResourceToText(RoomScript.Resources resource)
    {
        return resource switch
        {
            RoomScript.Resources.Energohoney => "Создаёт энергомёд",
            RoomScript.Resources.Supply      => "Подаёт питание",
            RoomScript.Resources.Cosmodrome  => "Добывает материалы",
            RoomScript.Resources.Research    => "Создаёт искусственные материалы",
            RoomScript.Resources.Bed         => "Улучшает медведей",
            RoomScript.Resources.Asteriy     => "Перерабатывает астерий",
            RoomScript.Resources.Build       => "Ремонтирует и улучшает комплексы",
            _ => "Что-то пошло не так",
        };
    }

    private string ResourceToEfficBearText(RoomScript.Resources resource)
    {
        return resource switch
        {
            RoomScript.Resources.Energohoney => "Пасечник",
            RoomScript.Resources.Supply      => "Программист",
            RoomScript.Resources.Cosmodrome  => "Первопроходец",
            RoomScript.Resources.Research    => "Биоинженер",
            RoomScript.Resources.Bed         => "Творец",
            RoomScript.Resources.Asteriy     => "Нет",
            RoomScript.Resources.Build       => "Конструктор",
            _ => "Что-то пошло не так",
        };
    }

    private string ResourceToRoomTitle(RoomScript.Resources resource)
    {
        return resource switch
        {
            RoomScript.Resources.Energohoney => "Комплекс выработки энергомеда",
            RoomScript.Resources.Supply      => "Комплекс снабжения",
            RoomScript.Resources.Cosmodrome  => "Космодром",
            RoomScript.Resources.Research    => "Комплекс исследований",
            RoomScript.Resources.Bed         => "Жилой комплекс",
            RoomScript.Resources.Asteriy     => "Комплекс переработки астерия",
            RoomScript.Resources.Build       => "Комплекс строительства",
            _ => "Что-то пошло не так",
        };
    }

    private string ResourceToRoomInfo(RoomScript.Resources resource)
    {
        return resource switch
        {
            RoomScript.Resources.Energohoney => "Позволяет вырабатывать энергомед. Для добычи энергомеда персонаж должен провзаимодействовать с комплексом. Улучшение увеличивает скорость выработки энергомеда. Эффективный специалист — пасечник.",
            RoomScript.Resources.Supply      => "Обеспечивает снабжение комплексов энергомедом и электричеством через трубопровод и кабель-менеджмент. Для прокладки кабель-менеджмента необходимо вручную проставить маршрут проводов. Постепенно трубопровод ломается, уменьшая радиус покрытия. Взаимодействие с комплексом восстанавливает трубопровод. Улучшение увеличивает скорость взаимодействия. Эффективный специалист — программист.",
            RoomScript.Resources.Cosmodrome  => "Используется для отправки сообщений и добычи астерия. Космодром добывает необработанный астерий, поставляемый на комплекс переработки астерия; если все комплексы переработки астерия заняты, то добыча астерия невозможна. С космодрома можно отправлять сообщения внешнему миру о необходимых ресурсах или медведях. Отправка сообщения потребляет энергомед. Улучшение космодрома ускоряет отправку сообщений и скорость добычи астерия. Эффективный специалист — первопроходец.",
            RoomScript.Resources.Research    => "Позволяет открывать различные технологии и улучшения. Для разблокировки новой технологии персонаж должен взаимодействовать с комплексом. Улучшение позволяет открывать новые технологии. Эффективный специалист — биоинженер.",
            RoomScript.Resources.Bed         => "Увеличивает максимальное количество персонала на базе. Взаимодействие с комплексом воодушевляет 3-ех случайных медведей, увеличивая эффективность их работы на 10%. Улучшение увеличивает скорость взаимодействия. Эффективный специалист — творец.",
            RoomScript.Resources.Asteriy     => "Перерабатывает поставляемый необработанный астерий с космодрома. Взаимодействие начинается автоматически, если есть необработанный астерий. Улучшение повышает скорость переработки 1 единицы астерия.",
            RoomScript.Resources.Build       => "Комплекс строительства увеличивает количество одновременных действий в режиме строительства. Взаимодействие с комплексом назначает ответственного рабочего, который будет выполнять работу. Улучшение увеличивает скорость выполнения работ. Эффективный специалист — конструктор.",
            _ => "Что-то пошло не так",
        };
    }

    public bool GetIsStatsActive()
    {
        return isStatsActive;
    }

    public TextMeshProUGUI GetLevelText()
    {
        return levelText;
    }

    public void SetRoomScript(RoomScript roomScript)
    {
        Debug.Log("Записали румскрипт");
        this.roomScript = roomScript;
    }
}
