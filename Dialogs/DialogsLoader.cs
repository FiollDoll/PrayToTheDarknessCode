using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogsLoader
{
    public List<Dialog> LoadDialogs()
    {
        TextAsset json = Resources.Load<TextAsset>("dialogues");
        if (json == null)
        {
            Debug.LogError("JSON file not found!");
            return new List<Dialog>();
        }

        DialogCollection dialogs = JsonUtility.FromJson<DialogCollection>(json.text);

        if (dialogs == null || dialogs.Dialogs == null)
        {
            Debug.LogError("Failed to parse JSON!");
            return new List<Dialog>();
        }

        return dialogs.Dialogs;
    }

    [System.Serializable]
    public class DialogCollection
    {
        public List<Dialog> Dialogs;
    }
}