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
    [SerializeField] private Sprite nullSprite;
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

        playerItems.RemoveAt(slot);
        ManageInventoryPanel(); // Закрытие
    }

    public void WatchItem(int id)
    {
        itemInfoMenu.gameObject.SetActive(!itemInfoMenu.gameObject.activeSelf);
        itemInfoMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = playerItems[id].name;
        itemInfoMenu.transform.Find("TextDescription").GetComponent<TextMeshProUGUI>().text = playerItems[id].description;
        int num = id;
        itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>().interactable = playerItems[num].canUse;
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
    public string activateNameDialog;
    public string questName;
    public bool questNextStep;
}