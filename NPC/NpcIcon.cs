using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class NpcIcon
{
    public Sprite standardIcon, happyIcon, angryIcon, sadIcon, scaryIcon, wonderIcon, confusionIcon, curseIcon;
    
    public Sprite ReturnIcon(Enums.IconMood iconMoodSelected)
    {
        return iconMoodSelected switch
        {
            Enums.IconMood.Standard => standardIcon,
            Enums.IconMood.Happy => happyIcon,
            Enums.IconMood.Sad => sadIcon,
            Enums.IconMood.Scary => scaryIcon,
            Enums.IconMood.Wonder => wonderIcon,
            Enums.IconMood.Confusion => confusionIcon,
            Enums.IconMood.Angry => angryIcon,
            Enums.IconMood.Curse => curseIcon
        };
    }
}