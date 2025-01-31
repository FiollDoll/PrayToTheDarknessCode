using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notebook : MonoBehaviour
{
    [Header("Записки игрока")] public List<Note> playerNotes = new List<Note>();

    [Header("Записки в игре")] [SerializeField]
    private Note[] gameNotes = new Note[0];

    private Dictionary<string, Note> _gameNotesDict = new Dictionary<string, Note>();

    [Foldout("Scene objects", true)] [SerializeField]
    private GameObject buttonNpc;

    [SerializeField] private GameObject buttonChoiceActiveQuest;

    [SerializeField] private GameObject notebookMenu;
    [SerializeField] private GameObject pageReadMain, pageReadNote, pageReadHuman;
    [SerializeField] private TextMeshProUGUI headerMain, noteMain;
    [SerializeField] private TextMeshProUGUI headerHuman, noteHuman;
    [SerializeField] private Image iconHuman;
    [SerializeField] private Button buttonExit;

    private AllScripts _scripts;

    private void Start()
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
        return _gameNotesDict.GetValueOrDefault(nameNote);
    }

    public void AddNote(string nameNote)
    {
        Note newNote = GetNote(nameNote);
        if (newNote == null) return; // Если пустой

        playerNotes.Add(newNote);
        _scripts.notifyManager.StartNewNoteNotify(newNote.name.text);
    }

    public void ReadNote(int num, int mode = 0)
    {
        pageReadNote.gameObject.SetActive(true);
        switch (mode)
        {
            case 0:
            {
                pageReadMain.gameObject.SetActive(true);
                headerMain.text = playerNotes[num].name.text;
                noteMain.text = playerNotes[num].description.text;
                if (!playerNotes[num].readed)
                    playerNotes[num].readed = true;
                break;
            }
            case 1:
            {
                pageReadMain.gameObject.SetActive(true);
                Quest selectedQuest = _scripts.questsSystem.activeQuests[num];
                headerMain.text = selectedQuest.name.text;
                noteMain.text = selectedQuest.description.text;
                if (selectedQuest.steps[selectedQuest.totalStep].name.text != "")
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
                headerHuman.text = _scripts.player.familiarNpc[num].nameOfNpc.text;
                noteHuman.text = _scripts.player.familiarNpc[num].description.text;
                iconHuman.sprite = _scripts.player.familiarNpc[num].GetStyleIcon(NpcIcon.IconMood.Standart);
                iconHuman.SetNativeSize();
                break;
        }
    }
}