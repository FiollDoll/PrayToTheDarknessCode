using UnityEngine;

public class DialogInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;
    public string NameQuestRequired;
    public string nameQuestRequired => NameQuestRequired;
    public int StageInter;
    public int stageInter => StageInter;

    [Header("Preferences")] public bool nextQuestStep { get; set; }
    public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;

    public void DoInteraction()
    {
        DialogsManager.Instance.ActivateDialog(gameObject.name);
    }
}