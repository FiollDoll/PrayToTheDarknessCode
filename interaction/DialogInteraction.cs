using System.Threading.Tasks;
using UnityEngine;

public class DialogInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string InterLabel => label.text;

    [Header("Requires")] public bool autoUse;
    public bool AutoUse => autoUse;

    [Header("Preferences")] public string itemNameToUse;
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
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == QuestStep.ActionToDo.Talk &&
               QuestsManager.Instance.GetTotalQuestStep().target == gameObject.name;
    }

    public async Task DoInteraction()
    {
        await DialogsManager.Instance.ActivateDialog(gameObject.name);
        if (questName != "" && CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}