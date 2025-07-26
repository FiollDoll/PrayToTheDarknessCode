using UnityEngine;

[CreateAssetMenu(fileName = "Quest")]
public class Quest: ScriptableObject
{
    public string questName;
    public QuestStep[] questSteps = new QuestStep[0];
}
