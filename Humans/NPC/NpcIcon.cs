using UnityEngine;

[System.Serializable]
public class NpcIcon
{
    public Sprite standartIcon, happyIcon, angryIcon, sadIcon, scaryIcon, wonderIcon, confusionIcon, curseIcon;
    
    public enum IconMood
    {
        Standart,
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
            IconMood.Standart => standartIcon,
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