using UnityEngine;

public class LocationInteraction : MonoBehaviour, IInteractable
{
    [Header("Label")] public LanguageSetting label;
    [SerializeField] private bool useLocationName;
    public string interLabel => label.text;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;

    [Header("Preferences")] public string locationName;
    public string spawnName;
    public bool destroyAfterInter;
    public string itemNameToUse;
    public string itemNameUse => itemNameToUse;
    public string questName { get; set; }
    public string QuestName => questName;

    private Location _location;

    public void Initialize()
    {
        _location = ManageLocation.Instance.GetLocation(locationName);
        if (useLocationName)
            label = _location.name;
    }

    public bool CanInteractByQuest()
    {
        return QuestsManager.Instance.GetTotalQuestStep() != null &&
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == QuestStep.ActionToDo.Move &&
               QuestsManager.Instance.GetTotalQuestStep().target == locationName;
    }

    public void DoInteraction()
    {
        if (_location.locked) return;

        // Пока вырезано
        //if (ManageLocation.Instance.GetLocation(gameObject.name).autoEnter &&
        //ManageLocation.Instance.totalLocation.autoEnter)
        ManageLocation.Instance.ActivateLocation(locationName, spawnName);

        if (CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}