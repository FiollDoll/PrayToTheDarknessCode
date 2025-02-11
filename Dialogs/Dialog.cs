using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class Dialog
{
    [Header("Main")] public string nameDialog;
    [ReadOnly] public bool read;

    public DialogStyle styleOfDialog;

    public enum DialogStyle
    {
        Main,
        SubMain,
        BigPicture
    };

    public StepBranch[] stepBranches = Array.Empty<StepBranch>(); // Ветки диалога
    private Dictionary<string, StepBranch> _stepBranchesDict = new Dictionary<string, StepBranch>();
    
    public Sprite bigPicture;
    
    [Header("Preference")] public bool moreRead;
    public bool canMove, canInter;
    public float mainPanelStartDelay; // Задержка перед появлением

    [Header("Actions after end")]
    public FastChangesController fastChanges;
    public bool darkAfterEnd;
    public Transform posAfterEnd;
    public int activateCutsceneStepAfterEnd = -1;

    public void UpdateBranchesDict()
    {
        foreach (StepBranch stepBranch in stepBranches)
            _stepBranchesDict.TryAdd(stepBranch.branchName, stepBranch);
    }

    public StepBranch FindBranch(string branchName)
    {
        return _stepBranchesDict.GetValueOrDefault(branchName);
    }
}

[Serializable]
public class StepBranch
{
    public string branchName;
    [Header("Main branch")] public DialogStep[] dialogSteps = Array.Empty<DialogStep>();
    [Header("After end of branch")] public DialogChoice[] choices = Array.Empty<DialogChoice>();
}

[Serializable]
public class DialogStep
{
    [Header("Main")] public Npc totalNpc;

    public LanguageSetting dialogText;
    public NpcIcon.IconMood iconMoodSelected;

    [Header("Preference")] public Transform cameraTarget;
    public bool cursedText;
    public float delayAfterNext;
    public int activateCutsceneStep = -1;
    public FastChangesController fastChanges;
    
    public Sprite icon => totalNpc.GetStyleIcon(iconMoodSelected);
}

[Serializable]
public class DialogChoice
{
    [Header("Main")] public string nameNewBranch;
    [ReadOnly] public bool read;
    public LanguageSetting textQuestion;

    [Header("Preference")] public bool moreRead;
}