using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "NPC")]
public class Npc : ScriptableObject
{
    [Header("Основная информация")] public string nameInWorld;

    public string nameOfNpc => PlayerPrefs.GetString("language") == "ru" ? ruName : enName;
    public string ruName, enName;

    public string description => PlayerPrefs.GetString("language") == "ru" ? ruDescription : enDescription;
    public string ruDescription, enDescription;

    public List<NpcStyle> styles = new List<NpcStyle>() { new NpcStyle("standard") };
    private Dictionary<string, NpcStyle> _stylesDict = new Dictionary<string, NpcStyle>();

    [Header("Настройки")] public bool canMeet;

    // Назначаются при старте
    [HideInInspector] public Animator animator;
    [HideInInspector] public NpcController npcController;

    public void UpdateNpcStyleDict()
    {
        foreach (NpcStyle style in styles)
            _stylesDict.Add(style.nameOfStyle, style);
    }

    public Sprite GetStyleIcon(NpcIcon.IconMood iconMood)
    {
        if (npcController && npcController.selectedStyle != "")
            return _stylesDict[npcController.selectedStyle].styleIcon.ReturnIcon(iconMood);
        return _stylesDict["standard"].styleIcon.ReturnIcon(iconMood);
    }
}