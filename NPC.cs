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
    public icons icon;
}

[System.Serializable]
public class icons
{
    public Sprite standartIcon, happyIcon, angryIcon, sadIcon, scaryIcon, wonderIcon, confusionIcon;
}
