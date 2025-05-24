using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "Note")]
public class Note: ScriptableObject
{
    public string gameName;
    [JsonIgnore] public LanguageSetting noteName;

    [JsonIgnore] public LanguageSetting description;
}