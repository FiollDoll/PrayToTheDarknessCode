using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeData
{
    public string guid;
    public Enums.DialogStyle styleOfDialog; // Только если начало диалога
    public bool moreRead;
    public bool canMove;
    public bool canInter;
    public bool darkAfterEnd;

    public string characterName;
    public Enums.IconMood mood;

    public string dialogTextRu, dialogTextEn;

    public Sprite bigPicture;
    public AudioClip stepSpeech;
    public bool cursedText;
    public float delayAfterNext;
    public int activateCutsceneStep = -1;
    public float mainPanelStartDelay;
    public string fastChangesName;

    public bool startNode;
    public bool endNode;
    public int choices = 1;
    public List<string> choiceOptions;
    public Rect nodePosition;

    public NodeData()
    {
        guid = Guid.NewGuid().ToString();
    }
}