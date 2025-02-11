using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    [Header("Main")] public string nameDialog;
    public bool read;

    public DialogStyle styleOfDialog;

    public enum DialogStyle
    {
        Main,
        SubMain,
        BigPicture
    };

    public Sprite bigPicture;

    [Header("Branches")] public List<StepBranch> stepBranches = new List<StepBranch>();

    private Dictionary<string, StepBranch> _stepBranchesDict = new Dictionary<string, StepBranch>();

    [Header("Preference")] public bool moreRead;
    public bool canMove, canInter;
    public float mainPanelStartDelay; // Задержка перед появлением

    [Header("Actions after end")] public FastChangesController fastChanges;
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

[System.Serializable]
public class StepBranch
{
    public string branchName;
    [Header("Main branch")] public DialogStep[] dialogSteps = new DialogStep[0];
    [Header("After end of branch")] public DialogChoice[] choices = new DialogChoice[0];

    public StepBranch()
    {
    }

    public StepBranch(string name)
    {
        branchName = name;
    }
}

[System.Serializable]
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

[System.Serializable]
public class DialogChoice
{
    [Header("Main")] public string nameNewBranch;
    public bool read;
    public LanguageSetting textQuestion;

    [Header("Preference")] public bool moreRead;
}