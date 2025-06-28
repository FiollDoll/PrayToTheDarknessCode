using System.Threading.Tasks;
using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string InterLabel => label.text;

    [Header("Requires")] public bool autoUse;
    public bool AutoUse => autoUse;

    [Header("Preferences")] public bool DarkAfterUse { get; set; }
    public string itemNameToUse;
    public string ItemNameUse => itemNameToUse;
    public string questName;
    public string QuestName => questName;

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

    public async Task DoInteraction()
    {
        if (DarkAfterUse)
            GameMenuManager.Instance.NoVisionForTime(1f);

        InventoryManager.Instance.AddItem(gameObject.name);
        Destroy(gameObject);

        if (questName != "" && CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}