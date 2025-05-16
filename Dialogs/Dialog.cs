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