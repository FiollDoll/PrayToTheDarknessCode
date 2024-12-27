using UnityEngine;

[System.Serializable]
public class Item
{
    public string nameInGame;
    public string nameRu, nameEn;
    public string name => PlayerPrefs.GetString("language") == "ru" ? nameRu : nameEn;


    [TextArea] public string descriptionRu, descriptionEn;
    public string description => PlayerPrefs.GetString("language") == "ru" ? descriptionRu : descriptionEn;


    [TextArea] public string activationTextRu, activationTextEn;
    public string activationText => PlayerPrefs.GetString("language") == "ru" ? activationTextRu : activationTextEn;

    public Sprite icon
    {
        get
        {
            if (iconRu != null) // iconEn дефолт
                return PlayerPrefs.GetString("language") == "ru" ? iconRu : iconEn;

            return iconEn;
        }
    }

    public Sprite iconRu, iconEn;
    public bool watchIconOnly;

    [Header("UseSettings")] public bool canUse;
    public bool removeAfterUse = true;
    public bool useInInventory;
    public bool useInCollider;

    [Header("UseInInventorySetting")] public string activateNameDialog;
    public string questName;
    public int questStage = -1;
    public bool questNextStep;
}