using UnityEngine;

[CreateAssetMenu(fileName = "Chapter")]
public class Chapter: ScriptableObject
{
    /// <summary>
    /// Системное название
    /// </summary>
    public string gameName;

    /// <summary>
    /// Визуальное название
    /// </summary>
    public string chapterName;
    
    public FastChangesController changesController;
}