using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")] [SerializeField] private Inventory inventory;

    [Header("Menu settings")] [SerializeField]
    private GameObject inventorySlotPrefab;
    [SerializeField] private TextMeshProUGUI newItemText;
    [SerializeField] private GameObject invMenu, itemInfoMenu;
    
    private GameObject _mainStyle, _onlyIconStyle;
    private TextMeshProUGUI _textItemName, _textItemDescription;
    private Image _iconInMain, _iconInOnly;
    private Button _buttonItemActivate;
    private AllScripts _scripts;

    public void Initialize()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();

        _mainStyle = itemInfoMenu.transform.Find("main").gameObject;
        _onlyIconStyle = itemInfoMenu.transform.Find("onlyIcon").gameObject;
        _textItemName = _mainStyle.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
        _textItemDescription = _mainStyle.transform.Find("TextDescription").GetComponent<TextMeshProUGUI>();
        _iconInMain = _mainStyle.transform.Find("ItemIcon").GetComponent<Image>();
        _iconInOnly = _onlyIconStyle.transform.Find("ItemIcon").GetComponent<Image>();
        _buttonItemActivate = itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>();
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
        inventory.AddItem(nameItem);
        StartCoroutine(ActivateNotify(inventory.GetGameItem(nameItem).name));
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

        if (itemInfoMenu.gameObject.activeSelf)
        {
            Item selectedItem = inventory.GetPlayerItem(id);
            _mainStyle.gameObject.SetActive(!selectedItem.watchIconOnly);
            _onlyIconStyle.gameObject.SetActive(selectedItem.watchIconOnly);
            if (selectedItem.watchIconOnly)
                _iconInOnly.sprite = selectedItem.icon;
            else
            {
                _textItemName.text = selectedItem.name;
                _textItemDescription.text =
                    selectedItem.description;
                _iconInMain.sprite = selectedItem.icon;
            }

            _buttonItemActivate.interactable = true; // Дефолт значение
            if (selectedItem.canUse && (selectedItem.useInInventory || selectedItem.useInCollider))
            {
                if (selectedItem.useInInventory)
                {
                    if (selectedItem.questName != "")
                    {
                        if (selectedItem.questName != _scripts.questsSystem.totalQuest.nameInGame
                            || (_scripts.questsSystem.totalQuest.totalStep != selectedItem.questStage &&
                                selectedItem.questStage > 0))
                            _buttonItemActivate.interactable = false;
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
                _buttonItemActivate.interactable = false;

            if (_buttonItemActivate.interactable)
            {
                _buttonItemActivate.onClick.AddListener(delegate { UseItem(id); });
                _buttonItemActivate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                    selectedItem.activationText != "" ? selectedItem.activationText : "???";
            }
            else
                _buttonItemActivate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    private void UpdateInvUI()
    {
        foreach (Transform child in invMenu.transform.Find("items"))
            Destroy(child.gameObject);

        for (int i = 0; i < inventory.CountPlayerItems(); i++)
        {
            Item selectedItem = inventory.GetPlayerItem(i);
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