using UnityEngine;

public class ChapterManager
{
    public static ChapterManager Instance { get; private set; }
    private Chapter[] _allChapters = new Chapter[0];
    private Chapter _selectedChapter;

    public void Initialize()
    {
        Instance = this;
        _allChapters = Resources.FindObjectsOfTypeAll<Chapter>();
    }

    public Chapter GetChapterByName(string name)
    {
        foreach (Chapter chapter in _allChapters)
        {
            if (chapter.gameName == name)
                return chapter;
        }

        return null;
    }
    
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