using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Cutscene")]
public class Cutscene: ScriptableObject
{
    public string cutsceneName;
    public CutsceneStep[] steps = new CutsceneStep[0];
}