using System.Threading.Tasks;
using UnityEngine;

public class SimpleInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string InterLabel => label.Text;

    [Header("Requires")] public bool autoUse;
    public bool AutoUse => autoUse;

    [Header("Preferences")] public FastChangesController changesController;

    public string itemNameToUse;
    public string ItemNameUse => itemNameToUse;
    
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
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == Enums.ActionToDo.Press &&
               QuestsManager.Instance.GetTotalQuestStep().target == gameObject;
    }

    public async Task DoInteraction()
    {
        await changesController.ActivateChanges();
        if (questName != "" && CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}