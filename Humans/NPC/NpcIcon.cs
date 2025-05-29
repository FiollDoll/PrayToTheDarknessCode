using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class NpcIcon
{
    public Sprite standardIcon, happyIcon, angryIcon, sadIcon, scaryIcon, wonderIcon, confusionIcon, curseIcon;

    public enum IconMood
    {
        Standard,
        Happy,
        Angry,
        Sad,
        Scary,
        Wonder, // Непонимание
        Confusion, // Удивление
        Curse
    }
    
    public Sprite ReturnIcon(IconMood iconMoodSelected)
    {
        return iconMoodSelected switch
        {
            IconMood.Standard => standardIcon,
            IconMood.Happy => happyIcon,
            IconMood.Sad => sadIcon,
            IconMood.Scary => scaryIcon,
            IconMood.Wonder => wonderIcon,
            IconMood.Confusion => confusionIcon,
            IconMood.Angry => angryIcon,
            IconMood.Curse => curseIcon
        };
    }
}