using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notebook : MonoBehaviour
{
    [SerializeField] private GameObject pageNote,
        pageQuest,
        pageNPC,
        pageNPCcontainer,
        pageChoiceNote,
        pageChoiceQuest,
        buttonChoiceActiveQuest;

    [SerializeField] private GameObject pageReadNote, pageReadHuman, pageReadMain;
    [SerializeField] private GameObject buttonNotePrefab, buttonNPC;
    [SerializeField] private GameObject newNoteNotify;
    [SerializeField] private Note[] gameNotes = new Note[0];
    [SerializeField] private List<Note> playerNotes = new List<Note>();
    private AllScripts _scripts;

    private void Start()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _scripts.notebook = this;
    }

    public void AddNote(string nameNote)
    {
        foreach (Note note in gameNotes)
        {
            if (nameNote == note.nameEn)
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

            for (int i = 0; i < _scripts.questsSystem.activeQuests.Count; i++)
            {
                var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity, pageChoiceQuest.transform);
                TextMeshProUGUI textName = obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
                if (_scripts.questsSystem.activeQuests[i] == _scripts.questsSystem.totalQuest)
                    textName.text = "-> " + _scripts.questsSystem.activeQuests[i].name;
                else
                    textName.text = _scripts.questsSystem.activeQuests[i].name;
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

            for (int i = 0; i < _scripts.player.familiarNPC.Count; i++)
            {
                var obj = Instantiate(buttonNPC, Vector3.zero, Quaternion.identity, pageNPCcontainer.transform);
                obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
                    _scripts.player.familiarNPC[i].nameOfNpc;
                obj.transform.Find("Icon").GetComponent<Image>().sprite =
                    _scripts.player.familiarNPC[i].icon.standartIcon;
                obj.transform.Find("Icon").GetComponent<Image>().SetNativeSize();
                int number = i;
                obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number, 2); });
            }

            pageNPC.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
        }
    }

    private void ReadNote(int num, int mode = 0)
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

        switch (mode)
        {
            case 0:
            {
                pageReadMain.gameObject.SetActive(true);
                header.text = playerNotes[num].name;
                note.text = playerNotes[num].description;
                if (!playerNotes[num].readed)
                    playerNotes[num].readed = true;
                break;
            }
            case 1:
            {
                pageReadMain.gameObject.SetActive(true);
                Quest selectedQuest = _scripts.questsSystem.activeQuests[num];
                header.text = selectedQuest.name;
                note.text = selectedQuest.description;
                if (selectedQuest.steps[selectedQuest.totalStep].name != "")
                    note.text += "\n\n -<b>" + selectedQuest.steps[selectedQuest.totalStep].name + "</b>\n" +
                                 selectedQuest.steps[selectedQuest.totalStep].description;
                if (selectedQuest != _scripts.questsSystem.totalQuest)
                {
                    buttonChoiceActiveQuest.gameObject.SetActive(true);
                    buttonChoiceActiveQuest.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        _scripts.questsSystem.ChoiceActiveQuest(selectedQuest.nameInGame);
                    });
                }
                else
                    buttonChoiceActiveQuest.gameObject.SetActive(false);

                break;
            }
            case 2:
                pageReadHuman.gameObject.SetActive(true);
                header.text = _scripts.player.familiarNPC[num].nameOfNpc;
                note.text = _scripts.player.familiarNPC[num].description;
                icon.sprite = _scripts.player.familiarNPC[num].icon.standartIcon;
                icon.SetNativeSize();
                break;
        }

        pageReadNote.transform.Find("ButtonExit").GetComponent<Button>().onClick
            .AddListener(delegate { ChoicePage(mode); });
    }

    private IEnumerator ActivateNotify()
    {
        newNoteNotify.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        newNoteNotify.gameObject.SetActive(false);
    }
}