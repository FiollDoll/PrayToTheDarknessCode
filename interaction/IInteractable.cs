public interface IInteractable
{
    // TODO: добавить auto use и другие условия в интеракциях
    // Главные настройки
    public string interLabel { get; } // PlayerPrefs.GetString("language") == "ru" ? ruLabelName : enLabelName

    // Общие настройки
    public bool autoUse { get; } // Автоматическое использование
    public string itemNameUse { get; } // Возможность использовать определенный предмет
    public string questName { get; }
    public void Initialize();
    public bool CanInteractByQuest();
    public void DoInteraction();
}