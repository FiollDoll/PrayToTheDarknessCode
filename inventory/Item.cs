using UnityEngine;

[System.Serializable]
public class Item
{
    public string nameInGame;
    public string nameRu, nameEn;
    [HideInInspector]
    public string name
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return nameRu;
            else
                return nameEn;
        }
    }
    public string descriptionRu, descriptionEn;
    [HideInInspector]
    public string description
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return descriptionRu;
            else
                return descriptionEn;
        }
    }
    public string activationTextRu, activationTextEn;
    [HideInInspector]
    public string activationText
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return activationTextRu;
            else
                return activationTextEn;
        }
    }
    [HideInInspector]
    public Sprite icon
    {
        get
        {
            if (iconRu != null) // iconEn дефолт
            {
                if (PlayerPrefs.GetString("language") == "ru")
                    return iconRu;
                else
                    return iconEn;
            }
            return iconEn;
        }
    }
    public Sprite iconRu, iconEn;
    public bool watchIconOnly;

    [Header("UseSettings")]
    public bool canUse;
    public bool removeAfterUse = true;
    public bool useInInventory;
    public bool useInCollider;

    [Header("UseInInventorySetting")]
    public string activateNameDialog;
    public string questName;
    public int questStage = -1;
    public bool questNextStep;
}