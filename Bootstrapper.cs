using System.Threading.Tasks;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private bool dev;
    [SerializeField] private Sprite nullSprite;
    [SerializeField] private GameObject startViewMenu, noViewPanel;

    private void Start() => InitializeGame();

    private async void InitializeGame()
    {
        noViewPanel.GetComponent<UnityEngine.UI.Image>().color = Color.black;

        // Мир
        await InitializeComponent(new DayProcess().Initialize());
        await InitializeComponent(new ManageLocation().Initialize());
        // UI + кор механики
        await InitializeComponent(new GameMenuManager().Initialize(
            GameObject.Find("Canvas").GetComponentsInChildren<IMenuable>(), startViewMenu, noViewPanel, nullSprite));
        await InitializeComponent(new QuestsManager().Initialize());
        await InitializeComponent(new NotesManager().Initialize());
        await InitializeComponent(new Inventory().Initialize());
        await InitializeComponent(new CameraManager().Initialize(Camera.main));
        await InitializeComponent(DialogUI.Instance.Initialize()); // 2 в 1

        // НПС
        await InitializeComponent(new NpcManager().Initialize());

        // Остальное
        await InitializeComponent(new SaveAndLoadManager().Initialize());
        await InitializeComponent(new ChapterManager().Initialize());

        GameMenuManager.Instance.DisableNoVision();

        if (!dev)
            await ChapterManager.Instance.StartLoadChapter(ChapterManager.Instance.GetChapterByName("prehistory")); // Потом сделать от сохранения
        else
            await ChapterManager.Instance.StartLoadChapter(ChapterManager.Instance.GetChapterByName("testChapter"));

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