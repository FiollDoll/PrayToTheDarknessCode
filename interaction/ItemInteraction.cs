using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;
    public string NameQuestRequired;
    public string nameQuestRequired => NameQuestRequired;
    public int StageInter;
    public int stageInter => StageInter;

    [Header("Preferences")] public bool darkAfterUse { get; set; }
    public bool nextQuestStep { get; set; }
    public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;

    public void DoInteraction()
    {
        if (darkAfterUse)
            Main.singleton.ActivateNoVision(1f);
        if (nextQuestStep)
            QuestsSystem.singleton.NextStep();

        InventoryManager.singleton.AddItem(gameObject.name);
        Destroy(gameObject);
    }
}