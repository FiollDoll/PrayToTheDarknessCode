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
    }

    public List<StepBranch> stepBranches = new List<StepBranch>();

    private Dictionary<string, StepBranch> _stepBranchesDict = new Dictionary<string, StepBranch>();

    [Header("Preference")]
    public bool moreRead;
    public bool canMove;
    public bool canInter;
    public float mainPanelStartDelay;

    [Header("Actions after end")]
    public bool darkAfterEnd;
    public Transform posAfterEnd;

    public Dialog()
    {
    }
    
    public void UpdateDialogDicts()
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
    public List<DialogStep> dialogSteps  = new List<DialogStep>();
    public List<DialogChoice> choices = new List<DialogChoice>();

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
    public string stepName;
    public string totalNpcName;
    public LanguageSetting dialogText;
    public string bigPictureName;
    public bool cursedText;
    public float delayAfterNext;
    public int activateCutsceneStep = -1;
    public FastChangesController fastChanges;
    public NpcIcon.IconMood iconMoodSelected;
    public Transform cameraTarget; // Сделать
    public Npc totalNpc;
    public Sprite icon;

    public void UpdateStep()
    {
        totalNpc = NpcManager.Instance.GetNpcByName(totalNpcName);
        icon = totalNpc?.GetStyleIcon(iconMoodSelected);
    }

    public Sprite GetBigPicture()
    {
        return Resources.Load<Sprite>("Cutscenes/" + bigPictureName);
    }
}

[System.Serializable]
public class DialogChoice
{
    [Header("Main")] public string nameNewBranch;
    public bool read;
    public LanguageSetting textQuestion;

    [Header("Preference")] public bool moreRead;
}