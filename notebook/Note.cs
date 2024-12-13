using UnityEngine;

[System.Serializable]
public class Note
{
    public string nameRu, nameEn;

    [HideInInspector]
    public string name
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? nameRu : nameEn; }
    }

    public string descriptionRu, descriptionEn;

    [HideInInspector]
    public string description
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? descriptionRu : descriptionEn; }
    }

    public bool readed;
}