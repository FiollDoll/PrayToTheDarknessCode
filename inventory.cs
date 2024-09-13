using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class inventory : MonoBehaviour
{
    public List<item> playerItems = new List<item>();
    [SerializeField] private item[] gameItems = new item[0];
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject itemInfoMenu;
    [SerializeField] private TextMeshProUGUI newItemText;
    [SerializeField] private allScripts scripts;

    public void ManageInventoryPanel()
    {
        transform.Find("inventoryMenu").gameObject.SetActive(!transform.Find("inventoryMenu").gameObject.activeSelf);
        itemInfoMenu.gameObject.SetActive(false);
        if (transform.Find("inventoryMenu").gameObject.activeSelf)
            UpdateInvUI();
    }

    public void AddItem(string name)
    {
        foreach (item item in gameItems)
        {
            if (item.nameEn == name)
            {
                playerItems.Add(item);
                StartCoroutine(ActivateNotify(item.name));
                break;
            }
        }
    }

    public void UseItem(int slot)
    {
        if (playerItems[slot].activateNameDialog != "")
            scripts.dialogsManager.ActivateDialog(playerItems[slot].activateNameDialog);
        if (playerItems[slot].questName != "" && playerItems[slot].questNextStep)
        {
            if (scripts.quests.FindQuest(playerItems[slot].questName) == scripts.quests.totalQuest)
                scripts.quests.NextStep();
        }

        if (playerItems[slot].removeAfterUse)
        {
            if (playerItems[slot].countUse > 0)
                playerItems[slot].countUse -= 1;

            if (playerItems[slot].countUse == 0)
                playerItems.RemoveAt(slot);

            if (playerItems[slot].countUse == -1)
                playerItems.RemoveAt(slot);
        }

        ManageInventoryPanel(); // Закрытие
    }

    public void WatchItem(int id)
    {
        itemInfoMenu.gameObject.SetActive(!itemInfoMenu.gameObject.activeSelf);
        itemInfoMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = playerItems[id].name;
        itemInfoMenu.transform.Find("TextDescription").GetComponent<TextMeshProUGUI>().text = playerItems[id].description;
        int num = id;
        if (playerItems[num].canUse)
        {
            if (playerItems[num].useInInventory)
                itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>().interactable = true;
            else if (playerItems[num].useInCollider)
            {
                if (scripts.interactions.selectedEI.itemNameUse == playerItems[num].nameEn)
                    itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>().interactable = true;
                else
                    itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>().interactable = false;
            }
            else
                itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>().interactable = false;
        }
        itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>().onClick.AddListener(delegate { UseItem(num); });
    }

    private void UpdateInvUI()
    {
        foreach (Transform child in transform.Find("inventoryMenu").transform.Find("items"))
            Destroy(child.gameObject);

        for (int i = 0; i < playerItems.Count; i++)
        {
            var obj = Instantiate(inventorySlotPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform.Find("inventoryMenu").transform.Find("items"));
            obj.transform.Find("item").GetComponent<Image>().sprite = playerItems[i].icon;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { WatchItem(num); });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ManageInventoryPanel();
    }

    private IEnumerator ActivateNotify(string name)
    {
        newItemText.gameObject.SetActive(true);
        newItemText.text = "+" + name;
        yield return new WaitForSeconds(2);
        newItemText.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class item
{
    public string nameRu, nameEn;
    [HideInInspector]
    public string name
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return nameRu;
            else
                return nameEn;
        }
    }
    public string descriptionRu, descriptionEn;
    [HideInInspector]
    public string description
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return descriptionRu;
            else
                return descriptionEn;
        }
    }
    public Sprite icon;

    [Header("UseSettings")]
    public bool canUse;
    public float countUse = -1;
    public bool removeAfterUse = true;
    public bool useInInventory;
    public bool useInCollider;

    [Header("UseInInventorySetting")]
    public string activateNameDialog;
    public string questName;
    public bool questNextStep;
}
