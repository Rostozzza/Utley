using UnityEngine;
using System.Collections.Generic;

public class BearStatusListController : MonoBehaviour
{
    public List<GameObject> bears;
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform content;
    public BearStatusController CreateBearStatus(UnitScript obj)
    {
        Bear model = obj.bearModel;
        GameObject instantiateBearPanel = Instantiate(panelPrefab, content);
        bears.Add(instantiateBearPanel);
        instantiateBearPanel.GetComponent<BearStatusController>().Init(obj.gameObject, model.Name, obj.level, obj.job, obj.isBearBusy, obj.GetWorkStr(), obj.avatar);
        return instantiateBearPanel.GetComponent<BearStatusController>();
    }

    public void ClearList()
    {
        foreach (BearStatusController panel in GetComponentsInChildren<BearStatusController>())
        {
            Destroy(panel.gameObject,0.1f);
        }
    }
}
