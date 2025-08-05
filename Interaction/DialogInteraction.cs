using System.Threading.Tasks;
using UnityEngine;

public class DialogInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string InterLabel => label.Text;
    public Dialog[] dialogsInteract = new Dialog[0];

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
        Dialog selectedDialog = null;
        foreach (Dialog dialog in dialogsInteract)
        {
            if (!DialogsManager.Instance.dialogsTempInfo[dialog].dialogHasRead && !DialogsManager.Instance.dialogsTempInfo[dialog].dialogLock)
            {
                selectedDialog = dialog;
                break;
            }
        }

        if (selectedDialog == null)
            selectedDialog = DialogsManager.Instance.GetDialog(gameObject.name);

        await DialogsManager.Instance.ActivateDialog(selectedDialog);

        if (giveItem != "")
            Inventory.Instance.AddItem(giveItem);
        if (questName != "" && CanInteractByQuest())
            QuestsManager.Instance.NextStep();
        if (DestroyAfterUse)
            Destroy(gameObject);
    }
}