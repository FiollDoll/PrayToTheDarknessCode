public interface IInteractable
{
    // TODO: добавить auto use и другие условия в интеракциях
    // Главные настройки
    public string interLabel { get; } // PlayerPrefs.GetString("language") == "ru" ? ruLabelName : enLabelName

    // Зависимости
    public string nameQuestRequired { get;}
    public int stageInter { get;}

    // Общие настройки
    public bool autoUse { get;} // Автоматическое использование
    public string itemNameUse { get; } // Возможность использовать определенный предмет
    public void DoInteraction();
}