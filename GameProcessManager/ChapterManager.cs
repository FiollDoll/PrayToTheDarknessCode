using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChapterManager
{
    public static ChapterManager Instance { get; private set; }
    private Chapter[] _allChapters = new Chapter[0];
    private readonly Dictionary<string, Chapter> chaptersDict = new Dictionary<string, Chapter>();
    private Chapter _selectedChapter;

    public Task Initialize()
    {
        Instance = this;
        _allChapters = Resources.LoadAll<Chapter>("Chapters/");
        foreach (Chapter chapter in _allChapters)
            chaptersDict.Add(chapter.gameName, chapter);
        return Task.CompletedTask;
    }

    public Chapter GetChapterByName(string name) => chaptersDict.GetValueOrDefault(name);

    public async Task StartLoadChapter(Chapter chapter)
    {
        _selectedChapter = chapter;
        if (chapter)
            await LoadChapter(chapter);
        else
            Debug.Log("Chapter don`t find");
    }

    private async Task LoadChapter(Chapter chapter)
    {
        GameMenuManager.Instance.ViewMenuActivate(chapter.chapterName.Text);
        await _selectedChapter.changesController?.ActivateChanges();
    }
}