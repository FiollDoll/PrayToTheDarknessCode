using System.Threading.Tasks;
using UnityEngine;

public class DialogInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string InterLabel => label.Text;

    [Header("Requires")] public bool autoUse;
    public bool AutoUse => autoUse;
    public bool destroyAfterUse;
    public bool DestroyAfterUse => destroyAfterUse;

    [Header("Preferences")] public string itemNameToUse;
    public string ItemNameUse => itemNameToUse;
    public string giveItem;
    public string questName;
    public string QuestName => questName;

    private void OnEnable() => Initialize();
    
    public void Initialize()
    {
        
    }
    
    public bool CanInteractByQuest()
    {
        if (questName == "")
            return true;
        return QuestsManager.Instance.GetTotalQuestStep() != null &&
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == Enums.ActionToDo.Talk &&
               QuestsManager.Instance.GetTotalQuestStep().target == gameObject;
    }

    public async Task DoInteraction()
    {
        await DialogsManager.Instance.ActivateDialog(gameObject.name);
        if (giveItem != "")
            Inventory.Instance.AddItem(giveItem);
        if (questName != "" && CanInteractByQuest())
                QuestsManager.Instance.NextStep();
        if (DestroyAfterUse)
            Destroy(gameObject);
    }
}