using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class notebook : MonoBehaviour
{
    [SerializeField] private GameObject noteMenu, pageNote, pageQuest, pageReadNote, pageChoiceNote, pageChoiceQuest;
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

            for (int i = 0; i < scripts.quests.recentQuests.Count; i++)
            {
                var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity, pageChoiceQuest.transform);
                obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = scripts.quests.recentQuests[i].name;
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
        if (mode == 0)
        {
            pageReadNote.transform.Find("TextHeader").GetComponent<TextMeshProUGUI>().text = playerNotes[num].name;
            pageReadNote.transform.Find("TextNote").GetComponent<TextMeshProUGUI>().text = playerNotes[num].description;
        }
        else
        {
            pageReadNote.transform.Find("TextHeader").GetComponent<TextMeshProUGUI>().text = scripts.quests.totalQuest.name;
            pageReadNote.transform.Find("TextNote").GetComponent<TextMeshProUGUI>().text = "";
            for (int i = 0; i < (scripts.quests.totalStep + 1); i++)
            {
                if (scripts.quests.totalQuest.steps[i].name != "")
                {
                    if (i == scripts.quests.totalStep)
                        pageReadNote.transform.Find("TextNote").GetComponent<TextMeshProUGUI>().text += "\n -<b>" + scripts.quests.totalQuest.steps[i].name + "</b>";
                    else
                        pageReadNote.transform.Find("TextNote").GetComponent<TextMeshProUGUI>().text += "\n -" + scripts.quests.totalQuest.steps[i].name;
                }
            }
        }
    }

    public void ManageNotePanel()
    {
        noteMenu.gameObject.SetActive(!noteMenu.gameObject.activeSelf);
        scripts.player.canMove = !noteMenu.gameObject.activeSelf;
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
