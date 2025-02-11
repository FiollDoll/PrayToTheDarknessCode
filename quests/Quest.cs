[System.Serializable]
public class Quest
{
    public string nameInGame;
    public LanguageSetting name;
    public LanguageSetting description;

    public Step[] steps = new Step[0];
    public int totalStep;
    public bool cursedText;
}

[System.Serializable]
public class Step
{
    public LanguageSetting name;
    public LanguageSetting description;

    public float delayNextStep;
    public string startDialog;
}