using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class notebook : MonoBehaviour
{
    [SerializeField] private GameObject noteMenu, pageNote, pageQuest, pageReadNote, pageChoiceNote, pageChoiceQuest, buttonChoiceActiveQuest;
    [SerializeField] private GameObject buttonNotePrefab;
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
        pageReadNote.gameObject.SetActive(false);
        pageChoiceNote.gameObject.SetActive(false);
        buttonChoiceActiveQuest.gameObject.SetActive(false);
        if (num == 0)
        {
            pageNote.gameObject.SetActive(true);
            pageChoiceNote.gameObject.SetActive(true);
            foreach (Transform child in pageChoiceNote.transform)
                Destroy(child.gameObject);

            for (int i = 0; i < playerNotes.Count; i++)
            {
                var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity, pageChoiceNote.transform);
                obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = playerNotes[i].name;
                int number = i;
                obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number); });
            }
        }
        else if (num == 1)
        {
            pageQuest.gameObject.SetActive(true);
            pageChoiceQuest.gameObject.SetActive(true);
            foreach (Transform child in pageChoiceQuest.transform)
                Destroy(child.gameObject);

            for (int i = 0; i < scripts.quests.activeQuests.Count; i++)
            {
                var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity, pageChoiceQuest.transform);
                if (scripts.quests.activeQuests[i] == scripts.quests.totalQuest)
                    obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = "-> " + scripts.quests.activeQuests[i].name;
                else
                    obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = scripts.quests.activeQuests[i].name;
                int number = i;
                obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number, 1); });
            }
        }
    }

    public void ReadNote(int num, int mode = 0)
    {
        pageReadNote.gameObject.SetActive(true);
        pageChoiceNote.gameObject.SetActive(false);
        pageChoiceQuest.gameObject.SetActive(false);
        TextMeshProUGUI header = pageReadNote.transform.Find("TextHeader").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI note = pageReadNote.transform.Find("TextNote").GetComponent<TextMeshProUGUI>();

        if (mode == 0)
        {
            header.text = playerNotes[num].name;
            note.text = playerNotes[num].description;
        }
        else
        {
            quest selectedQuest = scripts.quests.activeQuests[num];
            header.text = selectedQuest.name;
            note.text = selectedQuest.description;
            int totalStep = selectedQuest.totalStep;
            if (selectedQuest.steps[totalStep].name != "")
                note.text += "\n\n -<b>" + selectedQuest.steps[totalStep].name + "</b>\n" + selectedQuest.steps[totalStep].description;
            if (selectedQuest != scripts.quests.totalQuest)
            {
                buttonChoiceActiveQuest.gameObject.SetActive(true);
                buttonChoiceActiveQuest.GetComponent<Button>().onClick.AddListener(delegate { scripts.quests.ChoiceActiveQuest(selectedQuest.nameInGame); });
            }
            else
                buttonChoiceActiveQuest.gameObject.SetActive(false);
        }
        pageReadNote.transform.Find("ButtonExit").GetComponent<Button>().onClick.AddListener(delegate { ChoicePage(mode); });
    }

    public void ManageNotePanel()
    {
        noteMenu.gameObject.SetActive(!noteMenu.gameObject.activeSelf);
        scripts.player.canMove = !noteMenu.gameObject.activeSelf;
        if (noteMenu.gameObject.activeSelf)
            ChoicePage(0);
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
}
