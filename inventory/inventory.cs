using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventory : MonoBehaviour
{
    public List<item> playerItems = new List<item>();
    [SerializeField] private GameObject[] slots = new GameObject[3];
    [SerializeField] private item[] gameItems = new item[0];
    [SerializeField] private Sprite nullSprite;
    [SerializeField] private allScripts scripts;

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
        UpdateInvUI();
    }

    public void UseItem(int slot)
    {
        if (playerItems[slot].canUse)
        {
            if (playerItems[slot].nameEn == "Cigarette")
            {
                if (scripts.quests.totalQuest.questId == 0)
                    scripts.quests.NextStep();
                scripts.dialogsManager.ActivateDialog("CigaretteActivate");
            }
            playerItems.RemoveAt(slot);
        }
        UpdateInvUI();
    }

    public void UpdateInvUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if ((playerItems.Count - 1) >= i)
                slots[i].transform.Find("item").GetComponent<Image>().sprite = playerItems[i].icon;
            else
                slots[i].transform.Find("item").GetComponent<Image>().sprite = nullSprite;
        }
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
    public Sprite icon;
    public bool canUse;
}
