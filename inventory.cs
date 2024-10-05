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
        GameObject invMenu = transform.Find("inventoryMenu").gameObject;
        invMenu.SetActive(!invMenu.activeSelf);
        scripts.player.canMove = !invMenu.activeSelf;
        itemInfoMenu.SetActive(false);
        if (invMenu.activeSelf)
            UpdateInvUI();
    }

    public void AddItem(string name)
    {
        foreach (item item in gameItems)
        {
            if (item.nameInGame == name)
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
            playerItems.RemoveAt(slot);

        ManageInventoryPanel(); // Закрытие
    }

    public void WatchItem(int id)
    {
        itemInfoMenu.gameObject.SetActive(!itemInfoMenu.gameObject.activeSelf);
        itemInfoMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = playerItems[id].name;
        itemInfoMenu.transform.Find("TextDescription").GetComponent<TextMeshProUGUI>().text = playerItems[id].description;
        itemInfoMenu.transform.Find("ItemIcon").GetComponent<Image>().sprite = playerItems[id].icon;
        Button button = itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>();
        if (playerItems[id].canUse)
        {
            if (playerItems[id].useInInventory)
            {
                if (playerItems[id].questName != "")
                {
                    if (playerItems[id].questName == scripts.quests.totalQuest.nameInGame)
                    {
                        if (playerItems[id].questStage != -1)
                        {
                            if (scripts.quests.totalQuest.totalStep == playerItems[id].questStage)
                                button.interactable = true;
                        }
                        else
                            button.interactable = true;

                    }
                    else
                        button.interactable = false;
                }
                else
                    button.interactable = true;
            }
            else if (playerItems[id].useInCollider)
            {
                if (scripts.interactions.selectedEI.itemNameUse == playerItems[id].nameInGame)
                    button.interactable = true;
                else
                    button.interactable = false;
            }
        }
        else
            button.interactable = false;
        if (button.interactable)
        {
            button.onClick.AddListener(delegate { UseItem(id); });
            if (playerItems[id].activationText != "")
                button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = playerItems[id].activationText;
            else
                button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "???";
        }
        else
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
    }

    private void UpdateInvUI()
    {
        foreach (Transform child in transform.Find("inventoryMenu").transform.Find("items"))
            Destroy(child.gameObject);

        for (int i = 0; i < playerItems.Count; i++)
        {
            var obj = Instantiate(inventorySlotPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform.Find("inventoryMenu").transform.Find("items"));
            obj.GetComponent<Image>().sprite = playerItems[i].icon;
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
    public string nameInGame;
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
    public string activationTextRu, activationTextEn;
    [HideInInspector]
    public string activationText
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return activationTextRu;
            else
                return activationTextEn;
        }
    }
    public Sprite icon;

    [Header("UseSettings")]
    public bool canUse;
    public bool removeAfterUse = true;
    public bool useInInventory;
    public bool useInCollider;

    [Header("UseInInventorySetting")]
    public string activateNameDialog;
    public string questName;
    public int questStage = -1;
    public bool questNextStep;
}
