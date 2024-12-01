using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

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
        instantiateBearPanel.GetComponent<BearStatusController>().Init(obj.gameObject, model.Name, model.Level, model.Qualification, obj.isBearBusy);
        return instantiateBearPanel.GetComponent<BearStatusController>();
    }
}
