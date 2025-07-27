using UnityEngine;

[CreateAssetMenu(fileName="Item")]
public class Item : ScriptableObject
{
    [Header("MainInfo")]
    public string nameInGame;
    public Sprite icon;
    public LanguageSetting name;
    public LanguageSetting description;

    [Header("UseSettings")]
    public bool canUse;
    public LanguageSetting useText;
    public bool removeAfterUse = true;
    public bool useInInventory;
    public bool useInCollider;

    [Header("UseInInventorySetting")]
    public string activateNameDialog;
    public string questName;
    public int questStage = -1;
    public bool questNextStep;
}