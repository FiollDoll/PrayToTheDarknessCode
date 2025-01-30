using UnityEngine;

public class LocationInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public Language label;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;
    public string NameQuestRequired;
    public string nameQuestRequired => NameQuestRequired;
    public int StageInter;
    public int stageInter => StageInter;

    [Header("Preferences")] public string spawnName;
    public bool destroyAfterInter;
    public bool nextQuestStep;
    public string ItemNameToUse;
    public string itemNameUse => ItemNameToUse;

    private AllScripts _scripts;

    private void Awake()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    public void DoInteraction()
    {
        if (_scripts.manageLocation.GetLocation(gameObject.name).locked) return;
        
        // Пока вырезано
        //if (_scripts.manageLocation.GetLocation(gameObject.name).autoEnter &&
            //_scripts.manageLocation.totalLocation.autoEnter)
        _scripts.manageLocation.ActivateLocation(gameObject.name, spawnName);
    }
}