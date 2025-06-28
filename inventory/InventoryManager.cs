using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    [Header("Inventory")] public Inventory inventory;

    [Header("Prefabs")] [SerializeField] private GameObject inventorySlotPrefab;

    [SerializeField] private Button buttonItemActivate;

    [SerializeField] private GameObject invMenu, itemInfoMenu;
    [SerializeField] private GameObject mainView, onlyIconView;
    [SerializeField] private TextMeshProUGUI textItemName, textItemDescription;
    [SerializeField] private Image iconInMain, iconInOnly;

    private void Awake()
    {
        Instance = this;
    }

    public async Task Initialize()
    {
        inventory.UpdateGameItemsDict();
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
        Item newItem = inventory.AddItem(nameItem);
        NotifyManager.Instance.StartNewItemNotify(newItem.name.text);
    }

    /// <summary>
    /// Использование предмета из инвентаря
    /// </summary>
    /// <param name="slot">Индекс предмета</param>
    public void UseItem(int slot)
    {
        itemInfoMenu.gameObject.SetActive(false);
        UpdateInvUI();
        //Player.Instance.playerMenu.gameObject.SetActive(false);
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
            textItemName.text = selectedItem.name.text;
            textItemDescription.text =
                selectedItem.description.text;
            iconInMain.sprite = selectedItem.icon;
        }

        buttonItemActivate.gameObject.SetActive(true);
        if (selectedItem.canUse && (selectedItem.useInInventory || selectedItem.useInCollider))
        {
            if (selectedItem.useInCollider) // Только для enteredColliders
            {
                /*if (Interactions.Instance.EnteredInteractions == null ||
                    (Interactions.Instance.EnteredInteractions != null &&
                     Interactions.Instance.EnteredInteractions.itemNameUse != selectedItem.nameInGame))
                    buttonItemActivate.gameObject.SetActive(false);*/
            }
        }
        else
            buttonItemActivate.gameObject.SetActive(false);

        if (buttonItemActivate.gameObject.activeSelf)
        {
            buttonItemActivate.onClick.AddListener(delegate { UseItem(id); });
            buttonItemActivate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                selectedItem.useText.text != "" ? selectedItem.useText.text : "???";
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