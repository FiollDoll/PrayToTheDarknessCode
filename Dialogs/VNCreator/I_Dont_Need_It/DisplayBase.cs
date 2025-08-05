using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DisplayBase : MonoBehaviour
{
    [HideInInspector] public Dialog story;

    [HideInInspector] public DialogStepNode currentDialogStepNode;
    [HideInInspector] public bool lastNode;

    protected List<string> loadList = new List<string>();

    void Awake()
    {
        if (PlayerPrefs.GetString(GameSaveManager.currentLoadName) == string.Empty)
        {
            currentDialogStepNode = story.GetFirstNode();
            loadList.Add(currentDialogStepNode.guid);
        }
        else
        {
            loadList = GameSaveManager.Load();
            if (loadList == null || loadList.Count == 0)
            {
                currentDialogStepNode = story.GetFirstNode();
                loadList = new List<string>();
                loadList.Add(currentDialogStepNode.guid);
            }
            else
                currentDialogStepNode = story.GetCurrentNode(loadList[loadList.Count - 1]);
        }
    }

    public virtual void NextNode(int _choiceId)
    {
        if (!lastNode)
        {
            currentDialogStepNode = story.GetNextNode(currentDialogStepNode.guid, _choiceId);
            lastNode = currentDialogStepNode.endNode;
            loadList.Add(currentDialogStepNode.guid);
        }
    }

    protected void Save() => GameSaveManager.Save(loadList);
    
}