using UnityEngine;

public class CutsceneInteraction : MonoBehaviour, IInteractable
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
        return true;
    }

    public void DoInteraction()
    {
        CutsceneManager.Instance.ActivateCutscene(gameObject.name);
    }
}