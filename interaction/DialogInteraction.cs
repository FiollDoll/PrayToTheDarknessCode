using UnityEngine;

public class DialogInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;

    [Header("Preferences")] public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;

    public void DoInteraction()
    {
        DialogsManager.Instance.ActivateDialog(gameObject.name);
    }
}