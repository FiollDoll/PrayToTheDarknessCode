using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    //TODO: сделать через асинхронность
    private void Start()
    {
        Player.Instance.Initialize();
        NpcManager npcManager = new NpcManager();
        npcManager.Initialize();
        ManageLocation.Instance.Initialize();
        Notebook.Instance.Initialize();
        InventoryManager.Instance.Initialize();
        CameraManager.Instance.Initialize();
        ChapterManager chapterManager = new ChapterManager();
        chapterManager.Initialize();
        CutsceneManager.Instance.Initialize();
        DialogsManager.Instance.Initialize();
        SaveAndLoadManager saveAndLoad = new SaveAndLoadManager();
        saveAndLoad.Initialize();
        ChapterManager.Instance.LoadChapter(ChapterManager.Instance.GetChapterByName("prehistory"));
    }
}
