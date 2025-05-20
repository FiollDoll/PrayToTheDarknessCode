using UnityEngine;

[CreateAssetMenu(fileName = "Cutscene")]
public class Cutscene: ScriptableObject
{
    public string name;
    public CutsceneStep[] steps = new CutsceneStep[0];
}