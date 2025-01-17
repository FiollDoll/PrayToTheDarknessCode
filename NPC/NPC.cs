using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "NPC")]
public class Npc : ScriptableObject
{
    [Header("Основная информация")] public string nameInWorld;

    public string nameOfNpc => PlayerPrefs.GetString("language") == "ru" ? ruName : enName;
    public string ruName, enName;

    public string description => PlayerPrefs.GetString("language") == "ru" ? ruDescription : enDescription;
    [TextArea]public string ruDescription, enDescription;

    public List<NpcStyle> styles = new List<NpcStyle>() { new NpcStyle("standard") };
    private Dictionary<string, NpcStyle> _stylesDict = new Dictionary<string, NpcStyle>();

    [Header("Настройки")] public bool canMeet;

    // Назначаются при старте
    [HideInInspector] public Animator animator;
    [HideInInspector] public IHumanable NpcController;

    public void UpdateNpcStyleDict()
    {
        foreach (NpcStyle style in styles)
            _stylesDict.Add(style.nameOfStyle, style);
    }

    public NpcStyle GetNpcStyle(string styleName)
    {
        return _stylesDict[styleName];
    }
    
    public Sprite GetStyleIcon(NpcIcon.IconMood iconMood)
    {
        if (NpcController != null && NpcController.selectedStyle != "")
            return _stylesDict[NpcController.selectedStyle].styleIcon.ReturnIcon(iconMood);
        return _stylesDict["standard"].styleIcon.ReturnIcon(iconMood);
    }
}