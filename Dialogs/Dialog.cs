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
    [Header("Диалог")] public Npc totalNpc;

    public string text => PlayerPrefs.GetString("language") == "ru" ? ruText : enText;
    [TextArea] public string ruText, enText;
    public NpcIcon.IconMood iconMoodSelected;

    [Header("Настройки диалога")] public Transform cameraTarget;
    public bool cursedText;
    public float delayAfterNext;
    public int activatedCutsceneStep = -1;
    public string questStart;

    public Sprite icon => totalNpc.GetStyleIcon(iconMoodSelected);
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