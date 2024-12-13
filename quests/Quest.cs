using UnityEngine;

[System.Serializable]
public class Quest
{
    public string nameInGame;
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

    public Step[] steps = new Step[0];
    public int totalStep;
    public bool cursedText;
}

[System.Serializable]
public class Step
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

    public float delayNextStep;
    public string startDialog;
}