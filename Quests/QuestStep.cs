[System.Serializable]
public class QuestStep
{
    /// <summary>
    /// Что нужно для этого шага
    /// </summary>
    public enum ActionToDo
    {
        Talk,
        Press,
        Take,
        Move
    }

    public ActionToDo actionToDo;
    public string target;
    public FastChangesController changesAfterEnd;
}