using System.Collections.Generic;
using UnityEngine;

public class ChapterManager
{
    public static ChapterManager Instance { get; private set; }
    private Chapter[] _allChapters = new Chapter[0];
    private readonly Dictionary<string, Chapter> chaptersDict = new Dictionary<string, Chapter>();
    private Chapter _selectedChapter;

    public void Initialize()
    {
        Instance = this;
        _allChapters = Resources.LoadAll<Chapter>("Chapters/");
        foreach (Chapter chapter in _allChapters)
            chaptersDict.Add(chapter.gameName, chapter);
    }

    public Chapter GetChapterByName(string name) => chaptersDict.GetValueOrDefault(name);
    
    public void LoadChapter(Chapter chapter)
    {
        _selectedChapter = chapter;
        if (chapter)
        {
            CutsceneManager.Instance.StartViewMenuActivate(chapter.chapterName);
            _selectedChapter.changesController.ActivateChanges();
        }
        else
            Debug.Log("Chapter don`t find");
    }
}