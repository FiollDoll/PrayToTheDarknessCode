using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "NPC")]
public class Npc : ScriptableObject
{
    public string nameOfNpc => PlayerPrefs.GetString("language") == "ru" ? ruName : enName;

    public string ruName, enName;
    public string nameInWorld;
    public bool canMeet;
    [HideInInspector] public Animator animator;
    [HideInInspector] public NpcMovement npcMovement;

    public string description
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? ruDescription : enDescription; }
    }

    public string ruDescription, enDescription;

    [Header("Icons NPC")] public NpcIcons npcIcon;
}