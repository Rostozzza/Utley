using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ActionKeys
{
    None,
    Pause,
    Quit
}

public class InputController : MonoBehaviour
{
    [SerializeField] GameObject prefPrefab;
    static public Dictionary<ActionKeys, KeyCode> defaultKeyDict = new Dictionary<ActionKeys, KeyCode>()
    {
        { ActionKeys.None, KeyCode.None},
        { ActionKeys.Pause, KeyCode.Escape},
        { ActionKeys.Quit, KeyCode.Escape}
    };

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
        keyDict = PlayerPrefs.HasKey("key_dict") ? StringToKeyDict(PlayerPrefs.GetString("key_dict")) : defaultKeyDict;
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
            Enum.TryParse(keyAndValuePair[0], out ActionKeys key);
            Enum.TryParse(keyAndValuePair[1], out KeyCode value);

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
}