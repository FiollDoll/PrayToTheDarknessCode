using UnityEngine;

public class SimpleInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;
    public string NameQuestRequired;
    public string nameQuestRequired => NameQuestRequired;
    public int StageInter;
    public int stageInter => StageInter;

    [Header("Preferences")] public string dialogStart;
    public bool destroyAfterInter;

    public string playerVisual = "";
    public bool nextQuestStep { get; set; }
    public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;

    private AllScripts _scripts;

    private void Awake()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    public void DoInteraction()
    {
        if (dialogStart != "")
            _scripts.dialogsManager.ActivateDialog(dialogStart);

        if (playerVisual != "")
            _scripts.player.ChangeStyle(playerVisual);

        if (destroyAfterInter)
            Destroy(this.gameObject);
    }
}