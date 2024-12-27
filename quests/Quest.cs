using UnityEngine;

[System.Serializable]
public class Quest
{
    public string nameInGame;
    public string nameRu, nameEn;
    public string name => PlayerPrefs.GetString("language") == "ru" ? nameRu : nameEn;

    [TextArea]public string descriptionRu, descriptionEn;
    public string description => PlayerPrefs.GetString("language") == "ru" ? descriptionRu : descriptionEn;

    public Step[] steps = new Step[0];
    public int totalStep;
    public bool cursedText;
}

[System.Serializable]
public class Step
{
    public string nameRu, nameEn;
    public string name => PlayerPrefs.GetString("language") == "ru" ? nameRu : nameEn;

    [TextArea]public string descriptionRu, descriptionEn;
    public string description => PlayerPrefs.GetString("language") == "ru" ? descriptionRu : descriptionEn;

    public float delayNextStep;
    public string startDialog;
}