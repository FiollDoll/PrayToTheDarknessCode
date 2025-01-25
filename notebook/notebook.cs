using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notebook : MonoBehaviour
{
    [Header("Записки игрока")] [SerializeField]
    private List<Note> playerNotes = new List<Note>();

    [Header("Записки в игре")] [SerializeField]
    private Note[] gameNotes = new Note[0];

    private Dictionary<string, Note> _gameNotesDict = new Dictionary<string, Note>();
    [Header("Prefabs")] [SerializeField] private GameObject buttonNotePrefab;

    [Foldout("Scene objects", true)] [SerializeField]
    private GameObject buttonNpc;

    [SerializeField] private GameObject buttonChoiceActiveQuest;
    [SerializeField] private GameObject pageNpсСontainer, pageChoiceNote, pageChoiceQuest;

    [SerializeField] private GameObject notebookMenu;
    [SerializeField] private GameObject pageNote, pageQuest, pageNpc;
    [SerializeField] [CanBeNull] private GameObject pageReadMain, pageReadNote, pageReadHuman;
    [SerializeField] private TextMeshProUGUI headerMain, noteMain;
    [SerializeField] private TextMeshProUGUI headerHuman, noteHuman;
    [SerializeField] private Image iconHuman;
    [SerializeField] private Button buttonExit;

    private AllScripts _scripts;

    public void Initialize()
    {
        foreach (Note note in gameNotes)
            _gameNotesDict.TryAdd(note.gameName, note);

        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    /// <summary>
    /// Возврат записки по имени
    /// </summary>
    /// <param name="nameNote"></param>
    /// <returns></returns>
    private Note GetNote(string nameNote)
    {
        if (_gameNotesDict.TryGetValue(nameNote, out Note note))
            return note;

        return null;
    }

    public void AddNote(string nameNote)
    {
        Note newNote = GetNote(nameNote);
        if (newNote == null) return; // Если пустой

        playerNotes.Add(newNote);
        _scripts.notifyManager.StartNewNoteNotify(newNote.name);
    }

    public void ChoicePage(int num)
    {
        pageNote.gameObject.SetActive(false);
        pageQuest.gameObject.SetActive(false);
        pageNpc.gameObject.SetActive(false);
        pageReadNote.gameObject.SetActive(false);
        pageReadMain.gameObject.SetActive(false);
        pageReadHuman.gameObject.SetActive(false);
        buttonChoiceActiveQuest.gameObject.SetActive(false);
        switch (num)
        {
            case 0:
            {
                pageNote.gameObject.SetActive(true);
                foreach (Transform child in pageChoiceNote.transform)
                    Destroy(child.gameObject);

                for (int i = 0; i < playerNotes.Count; i++)
                {
                    var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity,
                        pageChoiceNote.transform);
                    if (playerNotes[i].readed)
                        obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = playerNotes[i].name;
                    else
                        obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
                            "(*)" + playerNotes[i].name;
                    int number = i;
                    obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number); });
                }

                pageNote.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
                break;
            }
            case 1:
            {
                pageQuest.gameObject.SetActive(true);
                foreach (Transform child in pageChoiceQuest.transform)
                    Destroy(child.gameObject);

                for (int i = 0; i < _scripts.questsSystem.activeQuests.Count; i++)
                {
                    var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity,
                        pageChoiceQuest.transform);
                    TextMeshProUGUI textName = obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
                    if (_scripts.questsSystem.activeQuests[i] == _scripts.questsSystem.totalQuest)
                        textName.text = "-> " + _scripts.questsSystem.activeQuests[i].name;
                    else
                        textName.text = _scripts.questsSystem.activeQuests[i].name;
                    int number = i;
                    obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number, 1); });
                }

                pageQuest.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
                break;
            }
            case 2:
            {
                pageNpc.gameObject.SetActive(true);
                foreach (Transform child in pageNpсСontainer.transform)
                    Destroy(child.gameObject);

                for (int i = 0; i < _scripts.player.familiarNpc.Count; i++)
                {
                    var obj = Instantiate(buttonNpc, Vector3.zero, Quaternion.identity, pageNpсСontainer.transform);
                    obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
                        _scripts.player.familiarNpc[i].nameOfNpc;
                    obj.transform.Find("Icon").GetComponent<Image>().sprite =
                        _scripts.player.familiarNpc[i].GetStyleIcon(NpcIcon.IconMood.Standart);
                    obj.transform.Find("Icon").GetComponent<Image>().SetNativeSize();
                    int number = i;
                    obj.GetComponent<Button>().onClick.AddListener(delegate { ReadNote(number, 2); });
                }

                pageNpc.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
                break;
            }
        }
    }

    private void ReadNote(int num, int mode = 0)
    {
        pageReadNote.gameObject.SetActive(true);
        switch (mode)
        {
            case 0:
            {
                pageReadMain.gameObject.SetActive(true);
                headerMain.text = playerNotes[num].name;
                noteMain.text = playerNotes[num].description;
                if (!playerNotes[num].readed)
                    playerNotes[num].readed = true;
                break;
            }
            case 1:
            {
                pageReadMain.gameObject.SetActive(true);
                Quest selectedQuest = _scripts.questsSystem.activeQuests[num];
                headerMain.text = selectedQuest.name;
                noteMain.text = selectedQuest.description;
                if (selectedQuest.steps[selectedQuest.totalStep].name != "")
                    noteMain.text += "\n\n -<b>" + selectedQuest.steps[selectedQuest.totalStep].name + "</b>\n" +
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
                headerHuman.text = _scripts.player.familiarNpc[num].nameOfNpc;
                noteHuman.text = _scripts.player.familiarNpc[num].description;
                iconHuman.sprite = _scripts.player.familiarNpc[num].GetStyleIcon(NpcIcon.IconMood.Standart);
                iconHuman.SetNativeSize();
                break;
        }

        buttonExit.onClick.AddListener(delegate { ChoicePage(mode); });
    }
}