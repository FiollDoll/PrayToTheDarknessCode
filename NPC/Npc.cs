using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "NPC")]
public class Npc : ScriptableObject
{
#if UNITY_EDITOR
    [ScriptableObjectIcon] public Sprite sprite; // спрайт для эдитора
#endif
    [Header("Main information")] public string nameInWorld;
    [JsonIgnore] public LanguageSetting nameOfNpc;
    [JsonIgnore] public LanguageSetting description;

    [JsonIgnore] public List<NpcStyle> styles = new List<NpcStyle>() { new NpcStyle("Standard") };
    private readonly Dictionary<string, NpcStyle> _stylesDict = new Dictionary<string, NpcStyle>();

    [Header("TempInfo")] public float relationshipWithPlayer;

    [Header("Preference")] [JsonIgnore] public bool canMeet;

    // Назначаются при старте
    [JsonIgnore] [HideInInspector] public IHumanable NpcController;

    public void OnEnable()
    {
        foreach (NpcStyle style in styles)
            _stylesDict.TryAdd(style.nameOfStyle, style);
    }

    public NpcStyle GetNpcStyle(string styleName) => _stylesDict.GetValueOrDefault(styleName);
    
    public Sprite GetStyleIcon(Enums.IconMood iconMood)
    {
        if (NpcController != null && NpcController.SelectedStyle != "")
            return _stylesDict[NpcController.SelectedStyle].styleIcon.ReturnIcon(iconMood);
        return _stylesDict["Standard"].styleIcon.ReturnIcon(iconMood);
    }
}