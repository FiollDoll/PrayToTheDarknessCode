using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Inventory _inventory = new Inventory();
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject invMenu, itemInfoMenu;
    private GameObject _mainStyle, _onlyIconStyle;
    [SerializeField] private TextMeshProUGUI newItemText;
    private AllScripts _scripts;

    public void Initialize()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    /// <summary>
    /// Управление показом инвентаря
    /// </summary>
    /// <param name="state"></param>
    public void ManageInventoryPanel(bool state)
    {
        invMenu.gameObject.SetActive(state);
        itemInfoMenu.gameObject.SetActive(false);
        if (state)
            UpdateInvUI();
    }

    public void AddItem(string nameItem)
    {
        _inventory.AddItem(nameItem);
        StartCoroutine(ActivateNotify(_inventory.GetGameItem(nameItem).name));
    }
    
    /// <summary>
    /// Использование предмета из инвентаря
    /// </summary>
    /// <param name="slot">Индекс предмета</param>
    public void UseItem(int slot)
    {

        itemInfoMenu.gameObject.SetActive(false);
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
        if (_mainStyle == null || _onlyIconStyle == null) // Первое присвоение
        {
            _mainStyle = itemInfoMenu.transform.Find("main").gameObject;
            _onlyIconStyle = itemInfoMenu.transform.Find("onlyIcon").gameObject;
        }
        
        if (itemInfoMenu.gameObject.activeSelf)
        {
            Item selectedItem = _inventory.GetPlayerItem(id);
            _mainStyle.gameObject.SetActive(!selectedItem.watchIconOnly);
            _onlyIconStyle.gameObject.SetActive(selectedItem.watchIconOnly);
            if (selectedItem.watchIconOnly)
                _onlyIconStyle.transform.Find("ItemIcon").GetComponent<Image>().sprite = selectedItem.icon;
            else
            {
                _mainStyle.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = selectedItem.name;
                _mainStyle.transform.Find("TextDescription").GetComponent<TextMeshProUGUI>().text =
                    selectedItem.description;
                _mainStyle.transform.Find("ItemIcon").GetComponent<Image>().sprite = selectedItem.icon;
            }

            Button button = itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>();
            button.interactable = true; // Дефолт значение
            if (selectedItem.canUse && (selectedItem.useInInventory || selectedItem.useInCollider))
            {
                if (selectedItem.useInInventory)
                {
                    if (selectedItem.questName != "")
                    {
                        if (selectedItem.questName != _scripts.questsSystem.totalQuest.nameInGame
                            || (_scripts.questsSystem.totalQuest.totalStep != selectedItem.questStage &&
                                selectedItem.questStage > 0))
                            button.interactable = false;
                    }
                }
                // Отложено.
                /*
                else if (selectedItem.useInCollider)
                {
                    if (_scripts.interactions.selectedInteraction == null
                        || (_scripts.interactions.selectedInteraction != null && _scripts.interactions.selectedInteraction.itemNameUse !=
                            selectedItem.nameInGame))
                        button.interactable = false;
                }
                */
            }
            else
                button.interactable = false;

            if (button.interactable)
            {
                button.onClick.AddListener(delegate { UseItem(id); });
                button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                    selectedItem.activationText != "" ? selectedItem.activationText : "???";
            }
            else
                button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    private void UpdateInvUI()
    {
        foreach (Transform child in invMenu.transform.Find("items"))
            Destroy(child.gameObject);

        for (int i = 0; i < _inventory.CountPlayerItems(); i++)
        {
            Item selectedItem = _inventory.GetPlayerItem(i);
            var obj = Instantiate(inventorySlotPrefab, new Vector3(0, 0, 0), Quaternion.identity,
                invMenu.transform.Find("items"));
            // TODO: добавить еще название
            obj.GetComponent<Image>().sprite = selectedItem.icon;
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