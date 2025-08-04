using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogStepNode
{
    // Node settings
    public string guid;
    public bool startNode;
    public bool endNode;
    public Rect nodePosition;

    /// Choices
    public int choices = 1;
    public List<string> choiceName;
    public List<string> choiceOptionsRu;
    public List<string> choiceOptionsEn;
    public List<bool> choiceLock;


    // Dialog step settings
    public Enums.DialogStyle styleOfDialog;
    public bool moreRead, canMove, canInter, darkAfterEnd;

    public Npc character;
    public Enums.IconMood mood;

    public string dialogTextRu, dialogTextEn;

    public Sprite bigPicture;
    public AudioClip stepSpeech;
    public float delayAfterNext;
    public float mainPanelStartDelay;

    // Changes
    public FastChangesController fastChanges;
    public Npc npcChangeRelation, npcChangeMoveToPlayer;
    public float changeRelation, changeKarma, changeSanity;
    public bool newStateMoveToPlayer, changeDialogLock;
    
    public Dialog dialogLock;
    public Dialog dialogChoiceLock;
    public string choiceNameToChange;
    public bool choiceNewState;

    public DialogStepNode()
    {
        guid = Guid.NewGuid().ToString();
    }
}