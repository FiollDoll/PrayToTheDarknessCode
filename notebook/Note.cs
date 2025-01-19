using UnityEngine;

[System.Serializable]
public class Note
{
    public string gameName;
    public string nameRu, nameEn;

    public string name => PlayerPrefs.GetString("language") == "ru" ? nameRu : nameEn;

    [TextArea] public string descriptionRu, descriptionEn;

    public string description => PlayerPrefs.GetString("language") == "ru" ? descriptionRu : descriptionEn;

    public bool readed;
}