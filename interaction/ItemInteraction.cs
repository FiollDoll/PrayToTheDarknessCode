using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;

    [Header("Preferences")] public bool darkAfterUse { get; set; }
    public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;

    public void DoInteraction()
    {
        if (darkAfterUse)
            GameMenuManager.Instance.ActivateNoVision(1f);

        InventoryManager.Instance.AddItem(gameObject.name);
        Destroy(gameObject);
    }
}