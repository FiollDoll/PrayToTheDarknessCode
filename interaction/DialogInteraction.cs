using UnityEngine;

public class DialogInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;

    [Header("Preferences")] public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;
    public string questName { get; set; }
    public string QuestName => questName;

    public bool CanInteractByQuest()
    {
        return QuestsManager.Instance.GetTotalQuestStep() != null &&
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == QuestStep.ActionToDo.Talk &&
               QuestsManager.Instance.GetTotalQuestStep().target == gameObject.name;
    }

    public void DoInteraction()
    {
        DialogsManager.Instance.ActivateDialog(gameObject.name);
        if (CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}