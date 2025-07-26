using System.Threading.Tasks;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private string chapterToLoad; // Начальная загрузка главы
    [SerializeField] private Sprite nullSprite;
    [SerializeField] private GameObject startViewMenu, noViewPanel;

    private void Start() => InitializeGame();

    private async void InitializeGame()
    {
        noViewPanel.GetComponent<UnityEngine.UI.Image>().color = Color.black;

        await InitializeComponent(new NpcManager().Initialize());
        await InitializeComponent(new ManageLocation().Initialize());
        await InitializeComponent(new GameMenuManager().Initialize(
            GameObject.Find("Canvas").GetComponentsInChildren<IMenuable>(), startViewMenu, noViewPanel, nullSprite));
        await InitializeComponent(new QuestsManager().Initialize());
        await InitializeComponent(new NotesManager().Initialize());
        await InitializeComponent(new Inventory().Initialize());
        await InitializeComponent(new CameraManager().Initialize(Camera.main));
        await InitializeComponent(new CutsceneManager().Initialize());
        await InitializeComponent(DialogUI.Instance.Initialize()); // 2 в 1
        await InitializeComponent(new DayProcess().Initialize());
        await InitializeComponent(new SaveAndLoadManager().Initialize());
        await InitializeComponent(new ChapterManager().Initialize());

        _ = GameMenuManager.Instance.DisableNoVision();

        ChapterManager.Instance.StartLoadChapter(ChapterManager.Instance.GetChapterByName(chapterToLoad));
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