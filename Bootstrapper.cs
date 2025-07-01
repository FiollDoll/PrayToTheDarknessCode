using System.Threading.Tasks;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private Sprite nullSprite;
    [SerializeField] private GameObject startViewMenu, noViewPanel;

    private void Start() => InitializeGame();

    private async void InitializeGame()
    {
        noViewPanel.GetComponent<UnityEngine.UI.Image>().color = Color.black;

        await InitializeComponent(Player.Instance.Initialize());
        await InitializeComponent(new NpcManager().Initialize());
        await InitializeComponent(new ManageLocation().Initialize());
        await InitializeComponent(new GameMenuManager().Initialize(
            GameObject.Find("Canvas").GetComponentsInChildren<IMenuable>(), startViewMenu, noViewPanel, nullSprite));
        await InitializeComponent(QuestsManager.Instance.Initialize());
        await InitializeComponent(new NotesManager().Initialize());
        await InitializeComponent(NotebookUI.Instance.Initialize(new NotesManager()));
        await InitializeComponent(InventoryManager.Instance.Initialize());
        await InitializeComponent(new CameraManager().Initialize(Camera.main));
        await InitializeComponent(new ChapterManager().Initialize());
        await InitializeComponent(new CutsceneManager().Initialize());
        await InitializeComponent(new DialogsManager().Initialize());
        await InitializeComponent(DialogUI.Instance.Initialize(DialogsManager.Instance));
        await InitializeComponent(new DayProcess().Initialize());
        await InitializeComponent(new SaveAndLoadManager().Initialize());

        _ = GameMenuManager.Instance.DisableNoVision();

# if UNITY_EDITOR
        ChapterManager.Instance.StartLoadChapter(ChapterManager.Instance.GetChapterByName("test"));
#elif UNITY_STANDALONE
        ChapterManager.Instance.StartLoadChapter(ChapterManager.Instance.GetChapterByName("prehistory"));
#endif
        Destroy(gameObject); // Самоуничтожение
    }

    /// <summary>
    /// Метод общей инициализации для того, чтобы поток не умирал с ошибко
    /// </summary>
    /// <param name="task"></param>
    private async Task InitializeComponent(Task task)
    {
        try
        {
            await task;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Инициализация завершилась ошибкой: {ex.Message}");
        }
    }
}