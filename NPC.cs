using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NPC", menuName = "NPC")]
public class NPC : ScriptableObject
{
    [HideInInspector]
    public string name
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return ruName;
            else
                return enName;
        }
    }
    public string ruName, enName;
    public string nameInWorld;

    [HideInInspector]
    public string description
    {
        get
        {
            if (!playerCloseMeet)
            {
                if (PlayerPrefs.GetString("language") == "ru")
                    return ruDescription;
                else
                    return enDescription;
            }
            else
            {
                if (PlayerPrefs.GetString("language") == "ru")
                    return ruCloseDescription;
                else
                    return enCloseDescription;
            }
        }
    }
    public string ruDescription, enDescription;

    [Header("-Closed description NPC")]
    public bool playerCloseMeet;
    public string ruCloseDescription, enCloseDescription;

    [Header("Icons NPC")]
    public icons icon;
}

[System.Serializable]
public class icons
{
    public Sprite standartIcon, happyIcon, angryIcon, sadIcon, scaryIcon, wonderIcon, confusionIcon, curseIcon;
}
