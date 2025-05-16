using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogStep
{
    public string totalNpcName;
    public LanguageSetting dialogText;
    public string bigPictureName;
    public string stepSpeech;
    public bool cursedText;
    public float delayAfterNext;
    public int activateCutsceneStep = -1;
    public FastChangesController fastChanges;
    public NpcIcon.IconMood iconMoodSelected;
    public Npc totalNpc;
    public Sprite icon;

    public void UpdateStep()
    {
        totalNpc = NpcManager.Instance.GetNpcByName(totalNpcName);
        icon = totalNpc?.GetStyleIcon(iconMoodSelected);
    }

    public Sprite GetBigPicture() => Resources.Load<Sprite>("Cutscenes/" + bigPictureName);
    
    public AudioClip GetSpeech() => Resources.Load<AudioClip>("Speech/" + stepSpeech);
    
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
public class DialogChoice
{
    [Header("Main")] public string nameNewBranch;
    public bool read;
    public LanguageSetting textQuestion;

    [Header("Preference")] public bool moreRead;
}