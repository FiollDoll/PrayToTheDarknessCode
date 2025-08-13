using System.Threading.Tasks;
using UnityEngine;

public class LocationInteraction : MonoBehaviour, IInteractable
{
    [Header("Label")] public LanguageSetting label;
    [SerializeField] private bool useLocationName;
    public string InterLabel => label.Text;

    [Header("Requires")] public bool autoUse;
    public bool AutoUse => autoUse;
    public bool destroyAfterUse;
    public bool DestroyAfterUse => destroyAfterUse;

    [Header("Preferences")] public string locationName;
    public string spawnName;
    public string itemNameToUse;
    public string ItemNameUse => itemNameToUse;
    public string questName;
    public string QuestName => questName;

    private Location _location;

    private void OnEnable() => Initialize();

    public void Initialize()
    {
        _location = ManageLocation.Instance?.GetLocation(locationName);
        if (useLocationName && _location)
            label = _location.name;
    }
    
    public bool CanInteractByQuest()
    {
        if (questName == "")
            return true;
        return QuestsManager.Instance.GetTotalQuestStep() != null &&
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == Enums.ActionToDo.Move &&
               QuestsManager.Instance.GetTotalQuestStep().target.name == locationName;
    }

    public async Task DoInteraction()
    {
        if (_location.locked) return;

        // Пока вырезано
        //if (ManageLocation.Instance.GetLocation(gameObject.name).autoEnter &&
        //ManageLocation.Instance.totalLocation.autoEnter)
        await ManageLocation.Instance.ActivateLocation(locationName, spawnName);

        if (questName != "" && CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}