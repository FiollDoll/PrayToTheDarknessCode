using UnityEngine;

[System.Serializable]
public class QuestStep
{
    public Enums.ActionToDo actionToDo;
    public GameObject target;
    public FastChangesController changesAfterEnd;
}