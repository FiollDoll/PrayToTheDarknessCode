using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    [Header("Main")] public string NameDialog;
    public bool Read;
    public DialogStyle StyleOfDialog;

    public enum DialogStyle
    {
        Main,
        SubMain,
        BigPicture
    }

    public List<StepBranch> StepBranches = new List<StepBranch>();

    private Dictionary<string, StepBranch> _stepBranchesDict = new Dictionary<string, StepBranch>();

    [Header("Preference")] public BigPicture[] BigPicturesPresets = new BigPicture[0];
    private Dictionary<string, BigPicture> _bigPicturePresetsDict = new Dictionary<string, BigPicture>();
    public bool MoreRead;
    public bool CanMove;
    public bool CanInter;
    public float MainPanelStartDelay;

    [Header("Actions after end")] public string FastChangesName;
    public bool DarkAfterEnd;
    public int ActivateCutsceneStepAfterEnd = -1;
    public Transform PosAfterEnd;

    public Dialog()
    {
    }
    
    public void UpdateDialogDicts()
    {
        foreach (StepBranch stepBranch in StepBranches)
            _stepBranchesDict.TryAdd(stepBranch.BranchName, stepBranch);

        foreach (BigPicture bigPicture in BigPicturesPresets)
            _bigPicturePresetsDict.Add(bigPicture.bigPictureName, bigPicture);
    }

    public StepBranch FindBranch(string branchName)
    {
        return _stepBranchesDict.GetValueOrDefault(branchName);
    }

    public BigPicture FindBigPicture(string bigPictureName)
    {
        return _bigPicturePresetsDict.GetValueOrDefault(bigPictureName);
    }
}

[System.Serializable]
public class StepBranch
{
    public string BranchName;
    public List<DialogStep> DialogSteps  = new List<DialogStep>();
    public List<DialogChoice> Choices = new List<DialogChoice>();

    public StepBranch()
    {
    }

    public StepBranch(string name)
    {
        BranchName = name;
    }
}

[System.Serializable]
public class DialogStep
{
    public string StepName;
    public string TotalNpcName;
    public LanguageSetting DialogText;
    public string BigPictureName;
    public bool CursedText;
    public float DelayAfterNext;
    public int ActivateCutsceneStep = -1;
    public string FastChangesName;
    public NpcIcon.IconMood IconMoodSelected;
    public Transform CameraTarget;
    public Npc totalNpc;
    public Sprite icon;

    public void UpdateStep()
    {
        totalNpc = Main.Instance.GetNpcByName(TotalNpcName);
        icon = totalNpc?.GetStyleIcon(IconMoodSelected);
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

[System.Serializable]
public class BigPicture
{
    public string bigPictureName;
    public Sprite[] sprites = new Sprite[0];
}