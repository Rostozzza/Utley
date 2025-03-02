using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ActionKeys
{
    None,
    Pause,
    Quit,
    Bear1,
    Bear2,
    Bear3,
    Bear4,
    Bear5,
    Bear6,
    OpenShop,
    //SelectUnitByPointer,
    MoveBearToRoom,
    BuildMode,
    InfoMode,
}

public class InputController : MonoBehaviour
{
    [SerializeField] GameObject prefPrefab;
    static public Dictionary<ActionKeys, KeyCode> defaultKeyDict = new Dictionary<ActionKeys, KeyCode>()
    {
        { ActionKeys.None, KeyCode.None},
        { ActionKeys.Pause, KeyCode.Escape},
        { ActionKeys.Quit, KeyCode.Escape},
        { ActionKeys.Bear1, KeyCode.Alpha1},
        { ActionKeys.Bear2, KeyCode.Alpha2},
        { ActionKeys.Bear3, KeyCode.Alpha3},
        { ActionKeys.Bear4, KeyCode.Alpha4},
        { ActionKeys.Bear5, KeyCode.Alpha5},
        { ActionKeys.Bear6, KeyCode.Alpha6},
        { ActionKeys.OpenShop, KeyCode.E},
        //{ ActionKeys.SelectUnitByPointer, KeyCode.Mouse0},
        { ActionKeys.MoveBearToRoom, KeyCode.Mouse1},
        { ActionKeys.BuildMode, KeyCode.B},
        { ActionKeys.InfoMode, KeyCode.I}
    };

    static private List<BindSelector> binds = new();

    static public Dictionary<ActionKeys, KeyCode> keyDict = defaultKeyDict;

    static public bool GetKeyDown(ActionKeys actionKey)
    {
        return Input.GetKeyDown(keyDict[actionKey]);
    }

    static public void ReceiveBindFromSelector(ActionKeys action, KeyCode keyCode)
    {
        keyDict[action] = keyCode;
        SaveKeyDict();
    }

    static private void SaveKeyDict()
    {
        PlayerPrefs.SetString("key_dict", KeyDictToSting(keyDict));
        PlayerPrefs.Save();
    }

    static public void LoadKeyDict()
    {
        keyDict = PlayerPrefs.HasKey("key_dict") ? StringToKeyDict(PlayerPrefs.GetString("key_dict")) : defaultKeyDict; // just trying to load;
        foreach (var key in defaultKeyDict.Keys) // if hasn't some actions, then add them from default;
        {
            if (!keyDict.ContainsKey(key))
            {
                keyDict.Add(key, defaultKeyDict[key]);
            }
        }
    }

    static private string KeyDictToSting(Dictionary<ActionKeys, KeyCode> keyDict)
    {
        string toReturn = "";
        foreach (var bind in keyDict)
        {
            toReturn += $"{bind.Key}@{bind.Value}|";
        }
        return toReturn[..^1];
    }

    static private Dictionary<ActionKeys, KeyCode> StringToKeyDict(string str)
    {
        Dictionary<ActionKeys, KeyCode> toReturn = new();

        List<string> keysAndValues = str.Split(new char[] { '|' }).ToList();
        foreach (var keyAndValue in keysAndValues)
        {
            List<string> keyAndValuePair = keyAndValue.Split(new char[] { '@' }).ToList();
            if (!Enum.TryParse(keyAndValuePair[0], out ActionKeys key)) continue;
            if (!Enum.TryParse(keyAndValuePair[1], out KeyCode value)) continue;

            toReturn.Add(key, value);
        }

        return toReturn;
    }

    static public Dictionary<ActionKeys, KeyCode> GetKeyDict() => keyDict;
    static public Dictionary<ActionKeys, KeyCode> GetDefaultKeyDict() => defaultKeyDict;

    private void CreateBindPrefs()
    {
        foreach (var key in keyDict.Keys)
        {
            if (key == ActionKeys.None) continue;
            var prefab = Instantiate(prefPrefab, transform);
            prefab.GetComponent<BindSelector>().SetAction(key);
        }
    }

    private void Start()
    {
        LoadKeyDict();
        CreateBindPrefs();
    }

    static public void AddBind(BindSelector bind)
    {
        binds.Add(bind);
    }

    static public void SolveBindConflicts()
    {
        binds.ForEach(bind => bind.SolveConflict());
    }

    static public void EraseAllBinds()
    {
        binds.ForEach(bind => bind.EraseBind());
    }
}