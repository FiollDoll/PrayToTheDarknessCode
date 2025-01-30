using UnityEngine;

public class CutsceneInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public Language label;
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
    
    private AllScripts _scripts;

    private void Awake()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    public void DoInteraction()
    {
        _scripts.cutsceneManager.ActivateCutscene(gameObject.name);
    }
}