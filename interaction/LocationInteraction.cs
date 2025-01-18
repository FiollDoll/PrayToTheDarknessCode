using UnityEngine;

public class LocationInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public string ruLabelName;
    public string enLabelName;
    public string interLabel => PlayerPrefs.GetString("language") == "ru" ? ruLabelName : enLabelName;

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
        if (!_scripts.manageLocation.GetLocation(this.gameObject.name).locked)
        {
            if (_scripts.manageLocation.GetLocation(this.gameObject.name).autoEnter &&
                _scripts.manageLocation.totalLocation.autoEnter)
                _scripts.manageLocation.ActivateLocation(this.gameObject.name, spawnName);
        }
    }
}