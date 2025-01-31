using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;
    public string NameQuestRequired;
    public string nameQuestRequired => NameQuestRequired;
    public int StageInter;
    public int stageInter => StageInter;

    [Header("Preferences")] public bool darkAfterUse { get; set; }
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
        if (darkAfterUse)
            _scripts.main.ActivateNoVision(1f);
        if (nextQuestStep)
            _scripts.questsSystem.NextStep();

        _scripts.inventoryManager.AddItem(gameObject.name);
        Destroy(gameObject);
    }
}