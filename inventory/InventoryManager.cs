using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")] [SerializeField] private Inventory inventory;

    [Header("Prefabs")] [SerializeField] private GameObject inventorySlotPrefab;

    [Foldout("SceneObjects", true)] [SerializeField]
    private Button buttonItemActivate;

    [SerializeField] private GameObject invMenu, itemInfoMenu;
    [SerializeField] private GameObject mainView, onlyIconView;
    [SerializeField] private TextMeshProUGUI textItemName, textItemDescription;
    [SerializeField] private Image iconInMain, iconInOnly;

    private AllScripts _scripts;

    private void Start()
    {
        inventory.UpdateGameItemsDict();
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    /// <summary>
    /// Открытие/закрытие инвентаря
    /// </summary>
    /// <param name="state"></param>
    public void ManageInventoryPanel(bool state)
    {
        invMenu.gameObject.SetActive(state);
        itemInfoMenu.gameObject.SetActive(false);
        if (state)
            UpdateInvUI();
    }

    /// <summary>
    /// Выдача предмета
    /// </summary>
    /// <param name="nameItem"></param>
    public void AddItem(string nameItem)
    {
        inventory.AddItem(nameItem);
        _scripts.notifyManager.StartNewItemNotify(inventory.GetGameItem(nameItem).name);
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
    /// Просмотр определенного предмета
    /// </summary>
    /// <param name="id"></param>
    private void WatchItem(int id)
    {
        itemInfoMenu.gameObject.SetActive(!itemInfoMenu.gameObject.activeSelf);

        if (!itemInfoMenu.gameObject.activeSelf) return; // Если меню закрылось

        Item selectedItem = inventory.GetPlayerItem(id);

        mainView.gameObject.SetActive(!selectedItem.watchIconOnly);
        onlyIconView.gameObject.SetActive(selectedItem.watchIconOnly);

        if (selectedItem.watchIconOnly)
            iconInOnly.sprite = selectedItem.icon;
        else
        {
            textItemName.text = selectedItem.name;
            textItemDescription.text =
                selectedItem.description;
            iconInMain.sprite = selectedItem.icon;
        }

        buttonItemActivate.gameObject.SetActive(true);
        if (selectedItem.canUse && (selectedItem.useInInventory || selectedItem.useInCollider))
        {
            if (selectedItem.useInInventory)
            {
                if (selectedItem.questName != "")
                {
                    if (selectedItem.questName != _scripts.questsSystem.totalQuest.nameInGame
                        || (_scripts.questsSystem.totalQuest.totalStep != selectedItem.questStage &&
                            selectedItem.questStage > 0))
                        buttonItemActivate.gameObject.SetActive(false);
                }
            }
            else if (selectedItem.useInCollider) // Только для enteredColliders
            {
                if (_scripts.interactions.EnteredInteraction == null ||
                    (_scripts.interactions.EnteredInteraction != null &&
                     _scripts.interactions.EnteredInteraction.itemNameUse != selectedItem.nameInGame))
                    buttonItemActivate.gameObject.SetActive(false);
            }
        }
        else
            buttonItemActivate.gameObject.SetActive(false);

        if (buttonItemActivate.gameObject.activeSelf)
        {
            buttonItemActivate.onClick.AddListener(delegate { UseItem(id); });
            buttonItemActivate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                selectedItem.useText != "" ? selectedItem.useText : "???";
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
}