using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    [SerializeField] private List<Item> playerItems;
    [SerializeField] private Item[] gameItems;

    public AllScripts scripts;

    public Item GetGameItem(string nameItem)
    {
        foreach (Item item in gameItems)
        {
            if (item.nameInGame != nameItem) continue;
            return item;
        }

        Debug.Log("Item not found!");
        return null;
    }

    public Item GetPlayerItem(int slot)
    {
        return playerItems[slot];
    }

    public int CountPlayerItems()
    {
        return playerItems.Count;
    }
    
    /// <summary>
    /// Выдача предмета
    /// </summary>
    /// <param name="nameItem">Название предмета</param>
    public void AddItem(string nameItem)
    {
        Item newItem = GetGameItem(nameItem);
        if (newItem != null)
            playerItems.Add(newItem);
    }

    /// <summary>
    /// Использование предмета, который лежит по индексу slot
    /// </summary>
    /// <param name="slot">Индекс предмета</param>
    public void UseItem(int slot)
    {
        if (playerItems[slot].activateNameDialog != "")
            scripts.dialogsManager.ActivateDialog(playerItems[slot].activateNameDialog);
        if (playerItems[slot].questName != "" && playerItems[slot].questNextStep)
        {
            if (scripts.questsSystem.FindQuest(playerItems[slot].questName) == scripts.questsSystem.totalQuest)
                scripts.questsSystem.NextStep();
        }

        if (playerItems[slot].removeAfterUse)
            playerItems.RemoveAt(slot);
    }
}