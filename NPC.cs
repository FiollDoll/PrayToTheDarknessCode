using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "NPC")]
public class NPC : ScriptableObject
{
    [HideInInspector]
    public string nameOfNpc
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? ruName : enName; }
    }

    public string ruName, enName;
    public string nameInWorld;

    [HideInInspector]
    public string description
    {
        get
        {
            if (!playerCloseMeet)
                return PlayerPrefs.GetString("language") == "ru" ? ruDescription : enDescription;
            else
                return PlayerPrefs.GetString("language") == "ru" ? ruCloseDescription : enCloseDescription;
        }
    }

    public string ruDescription, enDescription;

    [Header("-Closed description NPC")] public bool playerCloseMeet;
    public string ruCloseDescription, enCloseDescription;

    [Header("Icons NPC")] public Icons icon;
}

[System.Serializable]
public class Icons
{
    public Sprite standartIcon, happyIcon, angryIcon, sadIcon, scaryIcon, wonderIcon, confusionIcon, curseIcon;
}