﻿using System.Threading.Tasks;
using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public LanguageSetting label;
    public string InterLabel => label.Text;

    [Header("Requires")] public bool autoUse;
    public bool AutoUse => autoUse;

    [Header("Preferences")] public bool DarkAfterUse { get; set; }
    public string itemNameToUse;
    public string ItemNameUse => itemNameToUse;
    public string questName;
    public string QuestName => questName;

    private void OnEnable() => Initialize();
    
    public void Initialize()
    {
        
    }
    
    public bool CanInteractByQuest()
    {
        if (questName == "")
            return true;
        return QuestsManager.Instance.GetTotalQuestStep() != null &&
               QuestsManager.Instance.GetTotalQuestStep().actionToDo == Enums.ActionToDo.Take &&
               QuestsManager.Instance.GetTotalQuestStep().target == gameObject;
    }

    public async Task DoInteraction()
    {
        if (DarkAfterUse)
            GameMenuManager.Instance.NoVisionForTime(1f);

        Inventory.Instance.AddItem(gameObject.name);
        Destroy(gameObject);

        if (questName != "" && CanInteractByQuest())
            QuestsManager.Instance.NextStep();
    }
}