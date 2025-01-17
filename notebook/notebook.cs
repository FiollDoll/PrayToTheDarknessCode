using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notebook : MonoBehaviour
{
    [Header("Настройки")] [SerializeField] private GameObject newNoteNotify;
    [SerializeField] private GameObject buttonNotePrefab, buttonNpc;
    [SerializeField] private GameObject buttonChoiceActiveQuest, pageNpсСontainer, pageChoiceNote, pageChoiceQuest;

    private GameObject _notebookMenu;
    private GameObject _pageNote, _pageQuest, _pageNpc;
    private GameObject _pageReadNote, _pageReadHuman, _pageReadMain;
    private TextMeshProUGUI _headerMain, _noteMain;
    private TextMeshProUGUI _headerHuman, _noteHuman;
    private Image _iconHuman;
    private Button _buttonExit;

    [Header("Записки игрока")] [SerializeField]
    private List<Note> playerNotes = new List<Note>();

    [Header("Записки в игре")] [SerializeField]
    private Note[] gameNotes = new Note[0];

    private AllScripts _scripts;

    public void Initialize()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _notebookMenu = _scripts.player.playerMenu.transform.Find("notebookMenu").gameObject;
        newNoteNotify = GameObject.Find("newNote");
        _pageNote = _notebookMenu.transform.Find("pageNotes").gameObject;
        _pageQuest = _notebookMenu.transform.Find("pageQuests").gameObject;
        _pageNpc = _notebookMenu.transform.Find("pageNPC").gameObject;
        _pageReadNote = _notebookMenu.transform.Find("pageRead").gameObject;
        _pageReadHuman = _pageReadNote.transform.Find("human").gameObject;
        _pageReadMain = _pageReadNote.transform.Find("noteQuest").gameObject;

        _headerMain = _pageReadMain.transform.Find("TextHeader").GetComponent<TextMeshProUGUI>();
        _noteMain = _pageReadMain.transform.Find("TextNote").GetComponent<TextMeshProUGUI>();

        _headerHuman = _pageReadHuman.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
        _noteHuman = _pageReadHuman.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>();
        _iconHuman = _pageReadHuman.transform.Find("Icon").GetComponent<Image>();

        _buttonExit = _pageReadNote.transform.Find("ButtonExit").GetComponent<Button>();
    }

    public void AddNote(string nameNote)
    {
        foreach (Note note in gameNotes)
        {
            if (nameNote != note.nameEn) continue;
            playerNotes.Add(note);
            StartCoroutine(ActivateNotify());
            break;
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
            foreach (Transform child in pageNpсСontainer.transform)
                Destroy(child.gameObject);

            for (int i = 0; i < _scripts.player.familiarNpc.Count; i++)
            {
                var obj = Instantiate(buttonNpc, Vector3.zero, Quaternion.identity, pageNpсСontainer.transform);
                obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
                    _scripts.player.familiarNpc[i].nameOfNpc;
                obj.transform.Find("Icon").GetComponent<Image>().sprite =
                    _scripts.player.familiarNpc[i].npcIcon.standartIcon;
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
        switch (mode)
        {
            case 0:
            {
                _pageReadMain.gameObject.SetActive(true);
                _headerMain.text = playerNotes[num].name;
                _noteMain.text = playerNotes[num].description;
                if (!playerNotes[num].readed)
                    playerNotes[num].readed = true;
                break;
            }
            case 1:
            {
                _pageReadMain.gameObject.SetActive(true);
                Quest selectedQuest = _scripts.questsSystem.activeQuests[num];
                _headerMain.text = selectedQuest.name;
                _noteMain.text = selectedQuest.description;
                if (selectedQuest.steps[selectedQuest.totalStep].name != "")
                    _noteMain.text += "\n\n -<b>" + selectedQuest.steps[selectedQuest.totalStep].name + "</b>\n" +
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
                _headerHuman.text = _scripts.player.familiarNpc[num].nameOfNpc;
                _noteHuman.text = _scripts.player.familiarNpc[num].description;
                _iconHuman.sprite = _scripts.player.familiarNpc[num].npcIcon.standartIcon;
                _iconHuman.SetNativeSize();
                break;
        }

        _buttonExit.onClick.AddListener(delegate { ChoicePage(mode); });
    }

    private IEnumerator ActivateNotify()
    {
        newNoteNotify.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        newNoteNotify.gameObject.SetActive(false);
    }
}