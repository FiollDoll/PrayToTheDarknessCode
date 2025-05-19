using UnityEngine;

public class SimpleInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;

    [Header("Preferences")] public FastChangesController changesController;

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
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == QuestStep.ActionToDo.Press &&
               QuestsManager.Instance.GetTotalQuestStep().target == gameObject.name;
    }

    public void DoInteraction()
    {
        changesController.ActivateChanges();
        if (questName != "" && CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}