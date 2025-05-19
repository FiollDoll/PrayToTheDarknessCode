using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;

    [Header("Preferences")] public bool darkAfterUse { get; set; }
    public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;
    public string QuestName;
    public string questName => QuestName;

    public void Initialize()
    {
        
    }
    
    public bool CanInteractByQuest()
    {
        if (questName == "")
            return true;
        return QuestsManager.Instance.GetTotalQuestStep() != null &&
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == QuestStep.ActionToDo.Take &&
               QuestsManager.Instance.GetTotalQuestStep().target == gameObject.name;
    }

    public void DoInteraction()
    {
        if (darkAfterUse)
            GameMenuManager.Instance.ActivateNoVision(1f);

        InventoryManager.Instance.AddItem(gameObject.name);
        Destroy(gameObject);

        if (questName != "" && CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}