using UnityEngine;

public class LocationInteraction : MonoBehaviour, IInteractable
{
    [Header("Label")] public LanguageSetting label;
    [SerializeField] private bool useLocationName;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;
    public string NameQuestRequired;
    public string nameQuestRequired => NameQuestRequired;
    public int StageInter;
    public int stageInter => StageInter;

    [Header("Preferences")] 
    public string locationName;
    public string spawnName;
    public bool destroyAfterInter;
    public bool nextQuestStep;
    public string itemNameToUse;
    public string itemNameUse => itemNameToUse;

    private Location _location;
    private AllScripts _scripts;

    public void Initialize()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _location = _scripts.manageLocation.GetLocation(locationName);
        if (useLocationName)
            label = _location.name;
    }

    public void DoInteraction()
    {
        if (_location.locked) return;

        // Пока вырезано
        //if (_scripts.manageLocation.GetLocation(gameObject.name).autoEnter &&
        //_scripts.manageLocation.totalLocation.autoEnter)
        _scripts.manageLocation.ActivateLocation(locationName, spawnName);
    }
}