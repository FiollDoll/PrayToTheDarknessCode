using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

[System.Serializable]
public class Inventory
{
    public static Inventory Instance { get; private set; }
    [SerializeField] public List<Item> playerItems = new List<Item>();
    [SerializeField] private Item[] gameItems = new Item[0];
    private Dictionary<string, Item> _gameItemsDict = new Dictionary<string, Item>();

    public Task Initialize()
    {
        Instance = this;
        UpdateGameItemsDict();
        return Task.CompletedTask;
    }

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
    public Item GetGameItem(string nameItem) => _gameItemsDict.GetValueOrDefault(nameItem);
    

    /// <summary>
    /// Получить предмет в инвентаре игрока
    /// </summary>
    /// <param name="slot">Индекс слота</param>
    /// <returns></returns>
    public Item GetPlayerItem(int slot) => playerItems[slot];
    

    /// <summary>
    /// Получить число предметов в инвентаре
    /// </summary>
    /// <returns></returns>
    public int CountPlayerItems() => playerItems.Count;
    

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