using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [SerializeField] private GameObject invMenu, itemInfoMenu, itemsContainer;
    [SerializeField] private TextMeshProUGUI textItemName, textItemDescription;
    [SerializeField] private Image iconImage;

    [Header("Prefabs")][SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Button buttonItemActivate;

    private void Awake() => Instance = this;

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
        itemInfoMenu.SetActive(false);
        Inventory.Instance.UseItem(slot);
        ManageInventoryPanel(false);
        PlayerMenu.Instance.DisablePlayerMenu();
    }

    /// <summary>
    /// Просмотр определенного предмета
    /// </summary>
    /// <param name="id"></param>
    private void WatchItem(int id)
    {
        invMenu.SetActive(false);
        itemInfoMenu.SetActive(true);
        Item selectedItem = Inventory.Instance.GetPlayerItem(id);

        textItemName.text = selectedItem.name.Text;
        textItemDescription.text = selectedItem.description.Text;
        iconImage.sprite = selectedItem.icon;

        buttonItemActivate.gameObject.SetActive(selectedItem.canUse);

        if (buttonItemActivate.gameObject.activeSelf)
        {
            buttonItemActivate.onClick.AddListener(delegate { UseItem(id); });
            buttonItemActivate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                selectedItem.useText.Text != "" ? selectedItem.useText.Text : "???";
        }
    }

    public void DisableInfoMenu()
    {
        invMenu.SetActive(true);
        itemInfoMenu.SetActive(false);
    }

    private void UpdateInvUI()
    {
        foreach (Transform child in itemsContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < Inventory.Instance.CountPlayerItems(); i++)
        {
            Item selectedItem = Inventory.Instance.GetPlayerItem(i);
            var obj = Instantiate(inventorySlotPrefab, new Vector3(0, 0, 0), new Quaternion(), itemsContainer.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.GetComponentInChildren<TextMeshProUGUI>().text = selectedItem.name.Text;
            obj.GetComponent<Image>().sprite = selectedItem.icon;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { WatchItem(num); });
        }
    }
}