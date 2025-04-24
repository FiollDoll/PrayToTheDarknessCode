using UnityEngine;

public class CutsceneInteraction : MonoBehaviour, IInteractable
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
        return true;
    }

    public void DoInteraction()
    {
        CutsceneManager.Instance.ActivateCutscene(gameObject.name);
    }
}