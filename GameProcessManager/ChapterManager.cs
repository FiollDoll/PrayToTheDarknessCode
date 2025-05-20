using System.Collections;
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

    public void StartLoadChapter(Chapter chapter)
    {
        _selectedChapter = chapter;
        if (chapter)
            CoroutineContainer.Instance.StartCoroutine(LoadChapter(chapter));
        else
            Debug.Log("Chapter don`t find");
    }

    private IEnumerator LoadChapter(Chapter chapter)
    {
        Coroutine viewMenuCoroutine =
            CoroutineContainer.Instance.StartCoroutine(GameMenuManager.Instance.ViewMenuActivate(chapter.chapterName));
        yield return viewMenuCoroutine;
        _selectedChapter.changesController.ActivateChanges();
    }
}