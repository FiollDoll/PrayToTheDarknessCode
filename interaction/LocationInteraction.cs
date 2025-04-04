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

    public void Initialize()
    {
        _location = ManageLocation.Instance.GetLocation(locationName);
        if (useLocationName)
            label = _location.name;
    }

    public void DoInteraction()
    {
        if (_location.locked) return;

        // Пока вырезано
        //if (ManageLocation.Instance.GetLocation(gameObject.name).autoEnter &&
        //ManageLocation.Instance.totalLocation.autoEnter)
        ManageLocation.Instance.ActivateLocation(locationName, spawnName);
    }
}