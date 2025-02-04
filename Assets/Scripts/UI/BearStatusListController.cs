using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BearStatusListController : MonoBehaviour
{
    public List<GameObject> bears;
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform content;
    public BearStatusController CreateBearStatus(UnitScript obj, int priority)
    {
        Bear model = obj.bearModel;
        GameObject instantiateBearPanel = Instantiate(panelPrefab, content);
        bears.Add(instantiateBearPanel);
        instantiateBearPanel.GetComponent<BearStatusController>().Init(obj.gameObject, model.Name, obj.level, obj.job, obj.isBearBusy, obj.GetWorkStr(), obj.avatar, priority);
        return instantiateBearPanel.GetComponent<BearStatusController>();
    }

    private void Start()
    {
        Invoke("SortByPriority", 0.01f);
    }

    public void ClearList()
    {
        foreach (BearStatusController panel in GetComponentsInChildren<BearStatusController>())
        {
            Destroy(panel.gameObject);
        }
    }

    public void SortByPriority()
    {
        foreach (var bear in bears)
        {
            bear.transform.SetSiblingIndex(bear.GetComponent<BearStatusController>().GetPriority() - 1);
        }
    }
}
