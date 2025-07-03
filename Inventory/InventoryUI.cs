using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

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
    /// Использование предмета из инвентаря
    /// </summary>
    /// <param name="slot">Индекс предмета</param>
    public void UseItem(int slot)
    {
        itemInfoMenu.gameObject.SetActive(false);
        UpdateInvUI();
    }

    /// <summary>
    /// Просмотр определенного предмета
    /// </summary>
    /// <param name="id"></param>
    private void WatchItem(int id)
    {
        itemInfoMenu.gameObject.SetActive(!itemInfoMenu.gameObject.activeSelf);

        if (!itemInfoMenu.gameObject.activeSelf) return; // Если меню закрылось

        Item selectedItem = Inventory.Instance.GetPlayerItem(id);

        mainView.gameObject.SetActive(!selectedItem.watchIconOnly);
        onlyIconView.gameObject.SetActive(selectedItem.watchIconOnly);

        if (selectedItem.watchIconOnly)
            iconInOnly.sprite = selectedItem.icon;
        else
        {
            textItemName.text = selectedItem.name.Text;
            textItemDescription.text =
                selectedItem.description.Text;
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
                selectedItem.useText.Text != "" ? selectedItem.useText.Text : "???";
        }
    }

    private void UpdateInvUI()
    {
        foreach (Transform child in invMenu.transform.Find("items"))
            Destroy(child.gameObject);

        for (int i = 0; i < Inventory.Instance.CountPlayerItems(); i++)
        {
            Item selectedItem = Inventory.Instance.GetPlayerItem(i);
            var obj = Instantiate(inventorySlotPrefab, new Vector3(0, 0, 0), Quaternion.identity,
                invMenu.transform.Find("items"));
            // TODO: добавить еще название
            obj.GetComponent<Image>().sprite = selectedItem.icon;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { WatchItem(num); });
        }
    }
}