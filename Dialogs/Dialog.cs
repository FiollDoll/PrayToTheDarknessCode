﻿using System.Collections.Generic;
using UnityEngine;

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

    [Header("Branches")] public List<StepBranch> stepBranches = new List<StepBranch>();

    private Dictionary<string, StepBranch> _stepBranchesDict = new Dictionary<string, StepBranch>();

    [Header("Preference")] public BigPicture[] bigPicturesPresets = new BigPicture[0];
    private Dictionary<string, BigPicture> _bigPicturePresetsDict = new Dictionary<string, BigPicture>();
    public bool moreRead;
    public bool canMove, canInter;
    public float mainPanelStartDelay; // Задержка перед появлением

    [Header("Actions after end")] public string fastChangesName;
    public bool darkAfterEnd;
    public Transform posAfterEnd;
    public int activateCutsceneStepAfterEnd = -1;

    public void UpdateDialogDicts()
    {
        foreach (StepBranch stepBranch in stepBranches)
            _stepBranchesDict.TryAdd(stepBranch.branchName, stepBranch);

        foreach (BigPicture bigPicture in bigPicturesPresets)
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

public class DialogStep
{
    [Header("Main")] public static string totalNpcName;

    public LanguageSetting dialogText;
    public NpcIcon.IconMood iconMoodSelected;
    public string bigPictureName;

    [Header("Preference")] public Transform cameraTarget;
    public bool cursedText;
    public float delayAfterNext;
    public int activateCutsceneStep = -1;
    public string fastChangesName;

    public Npc totalNpc = Main.Instance.GetNpcByName(totalNpcName);
    public Sprite icon => totalNpc?.GetStyleIcon(iconMoodSelected);
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