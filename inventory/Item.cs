using UnityEngine;

[System.Serializable]
public class Item
{
    public string nameInGame;
    public string nameRu, nameEn;

    [HideInInspector]
    public string name
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? nameRu : nameEn; }
    }

    public string descriptionRu, descriptionEn;

    [HideInInspector]
    public string description
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? descriptionRu : descriptionEn; }
    }

    public string activationTextRu, activationTextEn;

    [HideInInspector]
    public string activationText
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? activationTextRu : activationTextEn; }
    }

    [HideInInspector]
    public Sprite icon
    {
        get
        {
            if (iconRu != null) // iconEn дефолт
            {
                return PlayerPrefs.GetString("language") == "ru" ? iconRu : iconEn;
            }

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