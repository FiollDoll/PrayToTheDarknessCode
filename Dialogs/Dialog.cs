using UnityEngine;

[System.Serializable]
public class Dialog
{
    [Header("Main")]
    public string nameDialog;
    public enum dialogStyle { main, subMain };
    public dialogStyle styleOfDialog;
    public bool canMove, canInter;
    [Tooltip("Обычные этапы диалога")] public DialogStep[] steps = new DialogStep[0];
    [Tooltip("Диалоги с выбором варианта. Начинается после steps")] public DialogStepChoice[] dialogsChoices = new DialogStepChoice[0];
    [Header("AfterEnd")]
    public string startNewDialogAfterEnd;
    public bool darkAfterEnd;
    [Tooltip("Работает только с darkAfterEnd")] public Transform posAfterEnd;
    [Header("Other")]
    public string noteAdd;
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
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return ruText;
            else
                return enText;
        }
    }
    public string ruText, enText;
    public bool cursedText;
    public bool animateTalking = true;
    public bool setCloseMeet;
    public float delayAfterNext;
    public enum iconMood { standart, happy, angry, sad, scary, wonder, confusion, curse }
    public iconMood iconMoodSelected;
    public Sprite icon
    {
        get
        {
            return iconMoodSelected switch
            {
                iconMood.standart => totalNpc.icon.standartIcon,
                iconMood.happy => totalNpc.icon.happyIcon,
                iconMood.sad => totalNpc.icon.sadIcon,
                iconMood.scary => totalNpc.icon.scaryIcon,
                iconMood.wonder => totalNpc.icon.wonderIcon,
                iconMood.confusion => totalNpc.icon.confusionIcon,
                iconMood.angry => totalNpc.icon.angryIcon,
                iconMood.curse => totalNpc.icon.curseIcon,
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
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return ruTextQuestion;
            else
                return enTextQuestion;
        }
    }
    public string ruTextQuestion, enTextQuestion;
    public DialogStep[] steps = new DialogStep[0];
    public bool readed, moreRead;
    public bool returnToStartChoices;
}