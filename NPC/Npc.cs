using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "NPC")]
public class Npc : ScriptableObject
{
#if UNITY_EDITOR
    [ScriptableObjectIcon] public Sprite sprite; // спрайт для эдитора
#endif

    [Header("Main information")]
    public string nameInWorld;
    public LanguageSetting nameOfNpc;
    public LanguageSetting specialRelationName;

    [Header("Other information")]
    public LanguageSetting profession;
    public LanguageSetting hobby;
    public int age;
    public LanguageSetting house;
    public LanguageSetting character;

    public List<NpcStyle> styles = new List<NpcStyle>() { new NpcStyle("Standard") };
    private readonly Dictionary<string, NpcStyle> _stylesDict = new Dictionary<string, NpcStyle>();

    [Header("Preference")] public bool canMeet;

    // Назначаются при старте
    [HideInInspector] public IHumanable IHumanable;

    public void OnEnable()
    {
        foreach (NpcStyle style in styles)
            _stylesDict.TryAdd(style.nameOfStyle, style);
    }

    public NpcStyle GetNpcStyle(string styleName) => _stylesDict.GetValueOrDefault(styleName);

    public Sprite GetStyleIcon(Enums.IconMood iconMood = Enums.IconMood.Standard)
    {
        if (IHumanable != null && IHumanable.SelectedStyle != "")
            return _stylesDict[IHumanable.SelectedStyle].styleIcon.ReturnIcon(iconMood);
        return _stylesDict["Standard"].styleIcon.ReturnIcon(iconMood);
    }
}