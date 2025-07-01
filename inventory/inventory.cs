using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    [SerializeField] public List<Item> playerItems = new List<Item>();
    [SerializeField] private Item[] gameItems = new Item[0];
    private Dictionary<string, Item> _gameItemsDict = new Dictionary<string, Item>();


    /// <summary>
    /// Инициализация словаря доступных предметов. Вызывается один раз!
    /// </summary>
    public void UpdateGameItemsDict()
    {
        foreach (Item item in gameItems)
            _gameItemsDict.TryAdd(item.nameInGame, item);
    }
    
    /// <summary>
    /// Получить Item по имени
    /// </summary>
    /// <param name="nameItem">Название предмета</param>
    /// <returns></returns>
    public Item GetGameItem(string nameItem)
    {
        return _gameItemsDict.GetValueOrDefault(nameItem);
    }

    /// <summary>
    /// Получить предмет в инвентаре игрока
    /// </summary>
    /// <param name="slot">Индекс слота</param>
    /// <returns></returns>
    public Item GetPlayerItem(int slot)
    {
        return playerItems[slot];
    }

    /// <summary>
    /// Получить число предметов в инвентаре
    /// </summary>
    /// <returns></returns>
    public int CountPlayerItems()
    {
        return playerItems.Count;
    }
    
    /// <summary>
    /// Выдача предмета
    /// </summary>
    /// <param name="nameItem">Название предмета</param>
    public Item AddItem(string nameItem)
    {
        Item newItem = GetGameItem(nameItem);
        if (newItem != null)
            playerItems.Add(newItem);
        return newItem;
    }

    /// <summary>
    /// Использование предмета, который лежит по индексу slot
    /// </summary>
    /// <param name="slot">Индекс предмета</param>
    public async void UseItem(int slot)
    {
        if (playerItems[slot].activateNameDialog != "")
            await DialogsManager.Instance.ActivateDialog(playerItems[slot].activateNameDialog);

        if (playerItems[slot].removeAfterUse)
            playerItems.RemoveAt(slot);
    }
}