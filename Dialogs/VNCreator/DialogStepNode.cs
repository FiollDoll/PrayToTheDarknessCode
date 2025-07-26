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
    public int choices = 1;
    public List<string> choiceOptionsRu;
    public List<string> choiceOptionsEn;
    public Rect nodePosition;

    // Dialog step settings
    public Enums.DialogStyle styleOfDialog;
    public bool moreRead, canMove, canInter, darkAfterEnd;

    public Npc character;
    public Enums.IconMood mood;

    public string dialogTextRu, dialogTextEn;

    public Sprite bigPicture;
    public AudioClip stepSpeech;
    public bool cursedText;
    public float delayAfterNext;
    public int activateCutsceneStep = -1;
    public float mainPanelStartDelay;
    public FastChangesController fastChanges;

    public DialogStepNode()
    {
        guid = Guid.NewGuid().ToString();
    }
}