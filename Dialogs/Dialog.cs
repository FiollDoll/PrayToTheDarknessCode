using UnityEngine;

[System.Serializable]
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
    public DialogStep[] steps = new DialogStep[0]; // Обычные этапы диалога
    public DialogStepChoice[] dialogsChoices = new DialogStepChoice[0]; // Этапы с выбором
    [Header("AfterEnd")] public string startNewDialogAfterEnd;
    public bool darkAfterEnd;

    [Tooltip("Работает только с darkAfterEnd")]
    public Transform posAfterEnd;

    [Header("Other")] public string noteAdd;
    public Sprite bigPicture, bigPictureSecond, bigPictureThird;
    public int activatedCutsceneStepAtEnd = -1;
    public bool disableFadeAtEnd;
    public float mainPanelStartDelay; // Задержка
    public bool nextStepQuest;
    public bool readed, moreRead;
}

[System.Serializable]
public class DialogStep
{
    public NPC totalNpc;

    [HideInInspector]
    public string text
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? ruText : enText; }
    }

    public string ruText, enText;
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

[System.Serializable]
public class DialogStepChoice
{
    [HideInInspector]
    public string textQuestion
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? ruTextQuestion : enTextQuestion; }
    }

    public string ruTextQuestion, enTextQuestion;
    public DialogStep[] steps = new DialogStep[0];
    public bool readed, moreRead;
    public bool returnToStartChoices;
}