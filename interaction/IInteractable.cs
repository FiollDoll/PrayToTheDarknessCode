using System.Threading.Tasks;

public interface IInteractable
{
    // TODO: добавить auto use и другие условия в интеракциях
    // Главные настройки
    public string InterLabel { get; }

    // Общие настройки
    public bool AutoUse { get; } // Автоматическое использование
    public string ItemNameUse { get; } // Возможность использовать определенный предмет
    public string QuestName { get; }
    public void Initialize();
    public bool CanInteractByQuest();
    public Task DoInteraction();
}