using System;
using UnityEngine;

[Serializable]
public class Dialog
{
    [Header("Main")] public string nameDialog;

    public enum DialogStyle
    {
        Main,
        SubMain
    };

    public DialogStyle styleOfDialog;
    public bool canMove, canInter;
    public StepBranch[] stepBranches = Array.Empty<StepBranch>(); // Обычные этапы диалога
    [Header("AfterEnd")] public string startNewDialogAfterEnd;
    public bool darkAfterEnd;

    [Tooltip("Работает только с darkAfterEnd")]
    public Transform posAfterEnd;

    [Header("Other")] public string noteAdd;
    public Sprite bigPicture;
    public int activatedCutsceneStepAtEnd = -1;
    public bool disableFadeAtEnd;
    public float mainPanelStartDelay; // Задержка
    public bool nextStepQuest;
    public bool readed, moreRead;

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
    [Header("Dialog")] public NPC totalNpc;

    public enum DialogMode
    {
        Dialog,
        Choice
    }

    public DialogMode dialogMode;

    public string text => PlayerPrefs.GetString("language") == "ru" ? ruText : enText;
    [TextArea] public string ruText, enText;

    public bool cursedText;
    public bool animateTalking = true;
    public bool setCloseMeet;
    public float delayAfterNext;

    public enum IconMood
    {
        Standart,
        Happy,
        Angry,
        Sad,
        Scary,
        Wonder,
        Confusion,
        Curse
    }

    public IconMood iconMoodSelected;

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

    public bool shakeIcon;
    public int activatedCutsceneStep = -1;
    public string questStart;
    public Transform cameraTarget;
}

[Serializable]
public class DialogChoice
{
    public string textQuestion => PlayerPrefs.GetString("language") == "ru" ? ruTextQuestion : enTextQuestion;
    public string ruTextQuestion, enTextQuestion;

    public string nameNewBranch;

    public bool readed, moreRead;
}