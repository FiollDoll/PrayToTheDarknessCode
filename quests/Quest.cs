using Newtonsoft.Json;

[System.Serializable]
public class Quest
{
    public string nameInGame;
    [JsonIgnore] public LanguageSetting name;
    [JsonIgnore] public LanguageSetting description;

    [JsonIgnore] public Step[] steps = new Step[0];
    public int totalStep;
    [JsonIgnore] public bool cursedText;
}

[System.Serializable]
public class Step
{
    public LanguageSetting name;
    public LanguageSetting description;

    public float delayNextStep;
    public string startDialog;
}