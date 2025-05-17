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
        
        ManageLocation.Instance.Initialize();
        yield return null;
        
        QuestsManager.Instance.Initialize();
        yield return null;
        
        Notebook.Instance.Initialize();
        yield return null;
        
        InventoryManager.Instance.Initialize();
        yield return null;
        
        CameraManager cameraManager = new CameraManager();
        cameraManager.Initialize(CoroutineContainer.Instance, Camera.main);
        yield return null;
        
        ChapterManager chapterManager = new ChapterManager();
        chapterManager.Initialize();
        yield return null;
        
        CutsceneManager.Instance.Initialize();
        yield return null;
        
        DialogsManager.Instance.Initialize();
        yield return null;
        
        DayProcess dayProcess = new DayProcess();
        dayProcess.Initialize(CoroutineContainer.Instance);
        yield return null;
        
        SaveAndLoadManager saveAndLoad = new SaveAndLoadManager();
        saveAndLoad.Initialize();
        yield return null;
        
        GameMenuManager.Instance.DisableNoVision();
        yield return null;
        ChapterManager.Instance.LoadChapter(ChapterManager.Instance.GetChapterByName("prehistory"));
    }
}