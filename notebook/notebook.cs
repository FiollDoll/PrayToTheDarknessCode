using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class Notebook : MonoBehaviour
{
    [Header("Настройки")] [SerializeField] private GameObject notebookMenu;

    private GameObject _pageNote, _pageQuest, _pageNpc;
    private GameObject _pageReadNote, _pageReadHuman, _pageReadMain;
    
    [SerializeField] private GameObject buttonChoiceActiveQuest, pageNpCcontainer, pageChoiceNote, pageChoiceQuest;
    [SerializeField] private GameObject buttonNotePrefab, buttonNPC;
    [SerializeField] private GameObject newNoteNotify;

    [Header("Записки игрока")] [SerializeField]
    private List<Note> playerNotes = new List<Note>();

    [Header("Записки в игре")] [SerializeField]
    private Note[] gameNotes = new Note[0];

    private AllScripts _scripts;

    private void Start()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _scripts.notebook = this;
        
        _pageNote = notebookMenu.transform.Find("pageNotes").gameObject;
        _pageQuest = notebookMenu.transform.Find("pageQuests").gameObject;
        _pageNpc = notebookMenu.transform.Find("pageNPC").gameObject;
        _pageReadNote = notebookMenu.transform.Find("pageRead").gameObject;
        _pageReadHuman = _pageReadNote.transform.Find("human").gameObject;
        _pageReadMain = _pageReadNote.transform.Find("noteQuest").gameObject;
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
        _pageNote.gameObject.SetActive(false);
        _pageQuest.gameObject.SetActive(false);
        _pageNpc.gameObject.SetActive(false);
        _pageReadNote.gameObject.SetActive(false);
        _pageReadMain.gameObject.SetActive(false);
        _pageReadHuman.gameObject.SetActive(false);
        buttonChoiceActiveQuest.gameObject.SetActive(false);
        if (num == 0)
        {
            _pageNote.gameObject.SetActive(true);
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

            _pageNote.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
        }
        else if (num == 1)
        {
            _pageQuest.gameObject.SetActive(true);
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

            _pageQuest.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
        }
        else if (num == 2)
        {
            _pageNpc.gameObject.SetActive(true);
            foreach (Transform child in pageNpCcontainer.transform)
                Destroy(child.gameObject);

            for (int i = 0; i < _scripts.player.familiarNPC.Count; i++)
            {
                var obj = Instantiate(buttonNPC, Vector3.zero, Quaternion.identity, pageNpCcontainer.transform);
                obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
                    _scripts.player.familiarNPC[i].nameOfNpc;
                obj.transform.Find("Icon").GetComponent<Image>().sprite =
                    _scripts.player.familiarNPC[i].icon.standartIcon;
                obj.transform.Find("Icon").GetComponent<Image>().SetNativeSize();
                int number = i;
                obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number, 2); });
            }

            _pageNpc.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
        }
    }

    private void ReadNote(int num, int mode = 0)
    {
        _pageReadNote.gameObject.SetActive(true);
        TextMeshProUGUI header = null;
        TextMeshProUGUI note = null;
        Image icon = null;
        if (mode == 0 || mode == 1)
        {
            header = _pageReadMain.transform.Find("TextHeader").GetComponent<TextMeshProUGUI>();
            note = _pageReadMain.transform.Find("TextNote").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            header = _pageReadHuman.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
            note = _pageReadHuman.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>();
            icon = _pageReadHuman.transform.Find("Icon").GetComponent<Image>();
        }

        switch (mode)
        {
            case 0:
            {
                _pageReadMain.gameObject.SetActive(true);
                header.text = playerNotes[num].name;
                note.text = playerNotes[num].description;
                if (!playerNotes[num].readed)
                    playerNotes[num].readed = true;
                break;
            }
            case 1:
            {
                _pageReadMain.gameObject.SetActive(true);
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
                _pageReadHuman.gameObject.SetActive(true);
                header.text = _scripts.player.familiarNPC[num].nameOfNpc;
                note.text = _scripts.player.familiarNPC[num].description;
                icon.sprite = _scripts.player.familiarNPC[num].icon.standartIcon;
                icon.SetNativeSize();
                break;
        }

        _pageReadNote.transform.Find("ButtonExit").GetComponent<Button>().onClick
            .AddListener(delegate { ChoicePage(mode); });
    }

    private IEnumerator ActivateNotify()
    {
        newNoteNotify.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        newNoteNotify.gameObject.SetActive(false);
    }
}