using System;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager Instance { get; private set; }
    [SerializeField] private Chapter[] allChapters = new Chapter[0];
    private Chapter _selectedChapter;
    
    private void Awake() => Instance = this;

    public Chapter GetChapterByName(string name)
    {
        foreach (Chapter chapter in allChapters)
        {
            if (chapter.gameName == name)
                return chapter;
        }

        return null;
    }
    
    public void LoadChapter(Chapter chapter)
    {
        _selectedChapter = chapter;
        _selectedChapter.changesController.ActivateChanges();
    }
}