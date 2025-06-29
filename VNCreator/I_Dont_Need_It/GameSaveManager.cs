using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameSaveManager
{
    public static string currentLoadName = string.Empty;

    public static List<string> Load(string loadName)
    {
        if (loadName == string.Empty)
        {
            currentLoadName = loadName;
            return null;
        }

        if (!PlayerPrefs.HasKey(currentLoadName))
        {
            Debug.LogError("You have not saved anything with the name " + currentLoadName);
            return null;
        }

        string _loadString = PlayerPrefs.GetString(currentLoadName);
        List<string> _loadList = _loadString.Split('_').ToList();
        _loadList.RemoveAt(_loadList.Count - 1);
        currentLoadName = loadName;
        return _loadList;
    }

    public static List<string> Load()
    {
        if (currentLoadName == string.Empty)
        {
            return null;
        }

        if (!PlayerPrefs.HasKey(currentLoadName))
        {
            Debug.LogError("You have not saved anything with the name " + currentLoadName);
            return null;
        }

        string _loadString = PlayerPrefs.GetString(currentLoadName);
        List<string> _loadList = _loadString.Split('_').ToList();
        return _loadList;
    }

    public static void Save(List<string> storyPath)
    {
        string _save = string.Join("_", storyPath.ToArray());
        PlayerPrefs.SetString(currentLoadName, _save);
    }

    public static void NewLoad(string saveName)
    {
        currentLoadName = saveName;
        PlayerPrefs.SetString(saveName, string.Empty);
    }
}