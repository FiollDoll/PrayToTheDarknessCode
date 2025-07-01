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
    [JsonIgnore] public Color nameColor;

    [JsonIgnore] public List<NpcStyle> styles = new List<NpcStyle>() { new NpcStyle("standard") };
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

    public NpcStyle GetNpcStyle(string styleName)
    {
        return _stylesDict.GetValueOrDefault(styleName);
    }

    public Sprite GetStyleIcon(NpcIcon.IconMood iconMood)
    {
        if (NpcController != null && NpcController.selectedStyle != "")
            return _stylesDict[NpcController.selectedStyle].styleIcon.ReturnIcon(iconMood);
        return _stylesDict["standard"].styleIcon.ReturnIcon(iconMood);
    }
}