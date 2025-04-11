using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager Instance { get; private set; }
    [SerializeField] private Chapter[] allChapters = new Chapter[0];
    private Chapter _selectedChapter;
    
    private void Awake() => Instance = this;
}