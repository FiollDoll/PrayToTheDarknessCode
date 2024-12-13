using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public List<Item> playerItems = new List<Item>();
    [SerializeField] private Item[] gameItems = new Item[0];
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject invMenu, itemInfoMenu, mainWatch, onlyIconWatch;
    [SerializeField] private TextMeshProUGUI newItemText;
    private AllScripts _scripts;

    private void Start()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _scripts.inventory = this;
    }

    /// <summary>
    /// Управление показом инвентаря
    /// </summary>
    /// <param name="state"></param>
    public void ManageInventoryPanel(bool state)
    {
        invMenu.gameObject.SetActive(state);
        if (state)
            UpdateInvUI();
    }

    /// <summary>
    /// Выдача предмета
    /// </summary>
    /// <param name="nameItem">Название предмета</param>
    public void AddItem(string nameItem)
    {
        foreach (Item item in gameItems)
        {
            if (item.nameInGame != nameItem) continue;
            playerItems.Add(item);
            StartCoroutine(ActivateNotify(item.name));
            break;
        }
    }

    /// <summary>
    /// Использование предмета из инвентаря
    /// </summary>
    /// <param name="slot">Индекс предмета</param>
    public void UseItem(int slot)
    {
        if (playerItems[slot].activateNameDialog != "")
            _scripts.dialogsManager.ActivateDialog(playerItems[slot].activateNameDialog);
        if (playerItems[slot].questName != "" && playerItems[slot].questNextStep)
        {
            if (_scripts.questsSystem.FindQuest(playerItems[slot].questName) == _scripts.questsSystem.totalQuest)
                _scripts.questsSystem.NextStep();
        }

        if (playerItems[slot].removeAfterUse)
            playerItems.RemoveAt(slot);

        WatchItem(-1); // Закрытие менюшки
        UpdateInvUI();
        _scripts.player.playerMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Просмотр определенного меню
    /// </summary>
    /// <param name="id"></param>
    private void WatchItem(int id)
    {
        itemInfoMenu.gameObject.SetActive(!itemInfoMenu.gameObject.activeSelf);
        if (itemInfoMenu.gameObject.activeSelf)
        {
            mainWatch.gameObject.SetActive(!playerItems[id].watchIconOnly);
            onlyIconWatch.gameObject.SetActive(playerItems[id].watchIconOnly);
            if (playerItems[id].watchIconOnly)
                onlyIconWatch.transform.Find("ItemIcon").GetComponent<Image>().sprite = playerItems[id].icon;
            else
            {
                mainWatch.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = playerItems[id].name;
                mainWatch.transform.Find("TextDescription").GetComponent<TextMeshProUGUI>().text =
                    playerItems[id].description;
                mainWatch.transform.Find("ItemIcon").GetComponent<Image>().sprite = playerItems[id].icon;
            }

            Button button = itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>();
            button.interactable = true; // Дефолт значение
            if (playerItems[id].canUse && (playerItems[id].useInInventory || playerItems[id].useInCollider))
            {
                if (playerItems[id].useInInventory)
                {
                    if (playerItems[id].questName != "")
                    {
                        if (playerItems[id].questName != _scripts.questsSystem.totalQuest.nameInGame
                            || (_scripts.questsSystem.totalQuest.totalStep != playerItems[id].questStage &&
                                playerItems[id].questStage > 0))
                            button.interactable = false;
                    }
                }
                else if (playerItems[id].useInCollider)
                {
                    if (_scripts.interactions.selectedEI == null
                        || (_scripts.interactions.selectedEI != null && _scripts.interactions.selectedEI.itemNameUse !=
                            playerItems[id].nameInGame))
                        button.interactable = false;
                }
            }
            else
                button.interactable = false;

            if (button.interactable)
            {
                button.onClick.AddListener(delegate { UseItem(id); });
                button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                    playerItems[id].activationText != "" ? playerItems[id].activationText : "???";
            }
            else
                button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    private void UpdateInvUI()
    {
        foreach (Transform child in invMenu.transform.Find("items"))
            Destroy(child.gameObject);

        for (int i = 0; i < playerItems.Count; i++)
        {
            var obj = Instantiate(inventorySlotPrefab, new Vector3(0, 0, 0), Quaternion.identity,
                invMenu.transform.Find("items"));
            obj.GetComponent<Image>().sprite = playerItems[i].icon;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { WatchItem(num); });
        }
    }

    private IEnumerator ActivateNotify(string notifyName)
    {
        newItemText.gameObject.SetActive(true);
        newItemText.text = "+" + notifyName;
        yield return new WaitForSeconds(2);
        newItemText.gameObject.SetActive(false);
    }
}