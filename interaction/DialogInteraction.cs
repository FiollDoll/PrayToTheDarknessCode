using UnityEngine;

public class DialogInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;

    [Header("Preferences")] public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;
    public string QuestName;
    public string questName => QuestName;

    public bool CanInteractByQuest()
    {
        if (questName == "")
            return true;
        return QuestsManager.Instance.GetTotalQuestStep() != null &&
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == QuestStep.ActionToDo.Talk &&
               QuestsManager.Instance.GetTotalQuestStep().target == gameObject.name;
    }

    public void DoInteraction()
    {
        DialogsManager.Instance.ActivateDialog(gameObject.name);
        if (questName != "" && CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}