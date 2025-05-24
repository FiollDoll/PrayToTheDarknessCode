using System.Collections;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private void Start() => StartCoroutine(InitializeGame());

    private IEnumerator InitializeGame()
    {
        GameMenuManager.Instance.noViewPanel.color = Color.black;
        
        Player.Instance.Initialize();
        yield return null;
        
        NpcManager npcManager = new NpcManager();
        npcManager.Initialize();
        yield return null;
        
        ManageLocation manageLocation = new ManageLocation();
        manageLocation.Initialize();
        yield return null;
        
        QuestsManager.Instance.Initialize();
        yield return null;

        NotesManager notesManager = new NotesManager();
        notesManager.Initialize();
        NotebookUI.Instance.Initialize(notesManager);
        yield return null;
        
        InventoryManager.Instance.Initialize();
        yield return null;
        
        CameraManager cameraManager = new CameraManager();
        cameraManager.Initialize(Camera.main);
        yield return null;
        
        ChapterManager chapterManager = new ChapterManager();
        chapterManager.Initialize();
        yield return null;

        CutsceneManager cutsceneManager = new CutsceneManager();
        cutsceneManager.Initialize();
        yield return null;

        DialogsManager dialogsManager = new DialogsManager();
        dialogsManager.Initialize();
        DialogUI.Instance.Initialize(dialogsManager);
        yield return null;
        
        DayProcess dayProcess = new DayProcess();
        dayProcess.Initialize();
        yield return null;
        
        SaveAndLoadManager saveAndLoad = new SaveAndLoadManager();
        saveAndLoad.Initialize();
        yield return null;
        
        GameMenuManager.Instance.DisableNoVision();
        yield return null;
        
        if (!DevConsole.Instance.devMode)
            ChapterManager.Instance.StartLoadChapter(ChapterManager.Instance.GetChapterByName("prehistory"));
        else
            ChapterManager.Instance.StartLoadChapter(ChapterManager.Instance.GetChapterByName("test"));
    }
}