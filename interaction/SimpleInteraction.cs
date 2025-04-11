using UnityEngine;

public class SimpleInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;

    [Header("Preferences")] public string dialogStart;
    public bool destroyAfterInter;

    public string playerVisual = "";
    public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;

    public void DoInteraction()
    {
        if (dialogStart != "")
            DialogsManager.Instance.ActivateDialog(dialogStart);

        if (playerVisual != "")
            Player.Instance.ChangeStyle(playerVisual);

        if (destroyAfterInter)
            Destroy(gameObject);
    }
}