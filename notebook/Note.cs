using Newtonsoft.Json;

[System.Serializable]
public class Note
{
    public string gameName;
    [JsonIgnore] public LanguageSetting name;

    [JsonIgnore] public LanguageSetting description;

    public bool read;
}