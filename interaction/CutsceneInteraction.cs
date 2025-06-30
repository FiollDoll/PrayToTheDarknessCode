using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class CutsceneInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string InterLabel => label.text;

    [Header("Requires")] public bool autoUse;
    public bool AutoUse => autoUse;

    [Header("Preferences")] public string itemNameToUse;
    public string ItemNameUse => itemNameToUse;
    public string questName;
    public string QuestName => questName;

    private void OnEnable() => Initialize();
    public void Initialize()
    {
        
    }
    
    public bool CanInteractByQuest()
    {
        return true;
    }

    public async Task DoInteraction()
    {
        await CutsceneManager.Instance.ActivateCutscene(gameObject.name);
    }
}