using UnityEngine;

[System.Serializable]
public class Item
{
    public string nameInGame;
    public LanguageSetting name;
    public LanguageSetting description;

    public Sprite icon
    {
        get
        {
            if (iconRu) // iconEn дефолт
                return PlayerPrefs.GetString("language") == "ru" ? iconRu : iconEn;

            return iconEn;
        }
    }

    public Sprite iconRu, iconEn;
    public bool watchIconOnly;

    [Header("UseSettings")] public bool canUse;
    public LanguageSetting useText;
    public bool removeAfterUse = true;
    public bool useInInventory;
    public bool useInCollider;

    [Header("UseInInventorySetting")] public string activateNameDialog;
    public string questName;
    public int questStage = -1;
    public bool questNextStep;
}