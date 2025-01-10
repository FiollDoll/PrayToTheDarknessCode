using System;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class Dialog
{
    [Header("Основное")] public string nameDialog;
    [ReadOnly] public bool readed;

    public DialogStyle styleOfDialog;

    public enum DialogStyle
    {
        Main,
        SubMain,
        BigPicture
    };

    public StepBranch[] stepBranches = Array.Empty<StepBranch>(); // Ветки диалога
    public Sprite bigPicture;
    [Header("Настройки")] public bool moreRead;
    public bool canMove, canInter;
    public float mainPanelStartDelay; // Задержка перед появлением

    [Header("Действия после окончания диалога")]
    public string noteAdd;

    public string startNewDialogAfterEnd;
    public bool darkAfterEnd;
    public Transform posAfterEnd;
    public bool nextStepQuest;
    public int activatedCutsceneStepAtEnd = -1;
    public bool disableFadeAtEnd; // TODO: сделать noviewPanel неперекрываемой, а после убрать эту переменную

    public StepBranch FindBranch(string branchName)
    {
        foreach (StepBranch stepBranch in stepBranches)
        {
            if (stepBranch.branchName == branchName)
                return stepBranch;
        }

        return null;
    }
}

[Serializable]
public class StepBranch
{
    public string branchName;
    [Header("Основная часть ветки")] public DialogStep[] dialogSteps = Array.Empty<DialogStep>();
    [Header("В конце ветки")] public DialogChoice[] choices = Array.Empty<DialogChoice>();
}

[Serializable]
public class DialogStep
{
    [Header("Диалог")] public NPC totalNpc;

    public string text => PlayerPrefs.GetString("language") == "ru" ? ruText : enText;
    [TextArea] public string ruText, enText;
    public IconMood iconMoodSelected;

    [Header("Настройки диалога")] public Transform cameraTarget;
    public bool cursedText;
    public bool setCloseMeet;
    public float delayAfterNext;
    public int activatedCutsceneStep = -1;
    public string questStart;

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

    public Sprite icon
    {
        get
        {
            return iconMoodSelected switch
            {
                IconMood.Standart => totalNpc.icon.standartIcon,
                IconMood.Happy => totalNpc.icon.happyIcon,
                IconMood.Sad => totalNpc.icon.sadIcon,
                IconMood.Scary => totalNpc.icon.scaryIcon,
                IconMood.Wonder => totalNpc.icon.wonderIcon,
                IconMood.Confusion => totalNpc.icon.confusionIcon,
                IconMood.Angry => totalNpc.icon.angryIcon,
                IconMood.Curse => totalNpc.icon.curseIcon,
            };
        }
    }
}

[Serializable]
public class DialogChoice
{
    [Header("Основное")] public string nameNewBranch;
    [ReadOnly] public bool readed;
    public string ruTextQuestion, enTextQuestion;
    public string textQuestion => PlayerPrefs.GetString("language") == "ru" ? ruTextQuestion : enTextQuestion;
    
    [Header("Настройки")] public bool moreRead;
}