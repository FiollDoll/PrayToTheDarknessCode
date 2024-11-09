using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class notebook : MonoBehaviour
{
    [SerializeField] private GameObject pageNote, pageQuest, pageNPC, pageNPCcontainer, pageChoiceNote, pageChoiceQuest, buttonChoiceActiveQuest;
    [SerializeField] private GameObject pageReadNote, pageReadHuman, pageReadMain;
    [SerializeField] private GameObject buttonNotePrefab, buttonNPC;
    [SerializeField] private GameObject newNoteNotify;
    [SerializeField] private note[] gameNotes = new note[0];
    [SerializeField] private List<note> playerNotes = new List<note>();
    [SerializeField] private allScripts scripts;


    public void AddNote(string name)
    {
        foreach (note note in gameNotes)
        {
            if (name == note.nameEn)
            {
                playerNotes.Add(note);
                StartCoroutine(ActivateNotify());
                break;
            }
        }
    }

    public void ChoicePage(int num)
    {
        pageNote.gameObject.SetActive(false);
        pageQuest.gameObject.SetActive(false);
        pageNPC.gameObject.SetActive(false);
        pageReadNote.gameObject.SetActive(false);
        pageReadMain.gameObject.SetActive(false);
        pageReadHuman.gameObject.SetActive(false);
        buttonChoiceActiveQuest.gameObject.SetActive(false);
        if (num == 0)
        {
            pageNote.gameObject.SetActive(true);
            foreach (Transform child in pageChoiceNote.transform)
                Destroy(child.gameObject);

            for (int i = 0; i < playerNotes.Count; i++)
            {
                var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity, pageChoiceNote.transform);
                if (playerNotes[i].readed)
                    obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = playerNotes[i].name;
                else
                    obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = "(*)" + playerNotes[i].name;
                int number = i;
                obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number); });
            }
            pageNote.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
        }
        else if (num == 1)
        {
            pageQuest.gameObject.SetActive(true);
            foreach (Transform child in pageChoiceQuest.transform)
                Destroy(child.gameObject);

            for (int i = 0; i < scripts.quests.activeQuests.Count; i++)
            {
                var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity, pageChoiceQuest.transform);
                TextMeshProUGUI textName = obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
                if (scripts.quests.activeQuests[i] == scripts.quests.totalQuest)
                    textName.text = "-> " + scripts.quests.activeQuests[i].name;
                else
                    textName.text = scripts.quests.activeQuests[i].name;
                int number = i;
                obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number, 1); });
            }
            pageQuest.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
        }
        else if (num == 2)
        {
            pageNPC.gameObject.SetActive(true);
            foreach (Transform child in pageNPCcontainer.transform)
                Destroy(child.gameObject);

            for (int i = 0; i < scripts.player.familiarNPC.Count; i++)
            {
                var obj = Instantiate(buttonNPC, Vector3.zero, Quaternion.identity, pageNPCcontainer.transform);
                obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = scripts.player.familiarNPC[i].name;
                obj.transform.Find("Icon").GetComponent<Image>().sprite = scripts.player.familiarNPC[i].icon.standartIcon;
                int number = i;
                obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number, 2); });
            }
            pageNPC.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
        }
    }

    public void ReadNote(int num, int mode = 0)
    {
        pageReadNote.gameObject.SetActive(true);
        TextMeshProUGUI header = null;
        TextMeshProUGUI note = null;
        Image icon = null;
        if (mode == 0 || mode == 1)
        {
            header = pageReadMain.transform.Find("TextHeader").GetComponent<TextMeshProUGUI>();
            note = pageReadMain.transform.Find("TextNote").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            header = pageReadHuman.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
            note = pageReadHuman.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>();
            icon = pageReadHuman.transform.Find("Icon").GetComponent<Image>();
        }
        if (mode == 0)
        {
            pageReadMain.gameObject.SetActive(true);
            header.text = playerNotes[num].name;
            note.text = playerNotes[num].description;
            if (!playerNotes[num].readed)
                playerNotes[num].readed = true;
        }
        else if (mode == 1)
        {
            pageReadMain.gameObject.SetActive(true);
            quest selectedQuest = scripts.quests.activeQuests[num];
            header.text = selectedQuest.name;
            note.text = selectedQuest.description;
            if (selectedQuest.steps[selectedQuest.totalStep].name != "")
                note.text += "\n\n -<b>" + selectedQuest.steps[selectedQuest.totalStep].name + "</b>\n" + selectedQuest.steps[selectedQuest.totalStep].description;
            if (selectedQuest != scripts.quests.totalQuest)
            {
                buttonChoiceActiveQuest.gameObject.SetActive(true);
                buttonChoiceActiveQuest.GetComponent<Button>().onClick.AddListener(delegate { scripts.quests.ChoiceActiveQuest(selectedQuest.nameInGame); });
            }
            else
                buttonChoiceActiveQuest.gameObject.SetActive(false);
        }
        else if (mode == 2)
        {
            pageReadHuman.gameObject.SetActive(true);
            header.text = scripts.player.familiarNPC[num].name;
            note.text = scripts.player.familiarNPC[num].description;
            icon.sprite = scripts.player.familiarNPC[num].icon.standartIcon;
        }
        pageReadNote.transform.Find("ButtonExit").GetComponent<Button>().onClick.AddListener(delegate { ChoicePage(mode); });
    }

    private IEnumerator ActivateNotify()
    {
        newNoteNotify.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        newNoteNotify.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class note
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
    public bool readed;
}
