using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notebook : MonoBehaviour
{
    public static Notebook Instance { get; private set; }
    [Header("Записки игрока")] public List<Note> playerNotes = new List<Note>();

    [Header("Записки в игре")] [SerializeField]
    private Note[] gameNotes = new Note[0];

    private Dictionary<string, Note> _gameNotesDict = new Dictionary<string, Note>();

    [Foldout("Scene objects", true)] [SerializeField]
    private GameObject buttonNpc;

    [SerializeField] private GameObject notebookMenu;
    [SerializeField] private GameObject pageReadMenu, pageReadNoteOrQuest, pageReadHuman;
    [SerializeField] private TextMeshProUGUI headerMain, noteMain;
    [SerializeField] private TextMeshProUGUI headerHuman, noteHuman, relationshipTextHuman;
    [SerializeField] private Slider sliderHuman;
    [SerializeField] private Image iconHuman;
    [SerializeField] private Button buttonExit;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (Note note in gameNotes)
            _gameNotesDict.TryAdd(note.gameName, note);
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
        NotifyManager.Instance.StartNewNoteNotify(newNote.name.text);
    }

    public void ReadNote(int num, int mode = 0)
    {
        pageReadMenu.gameObject.SetActive(true);
        switch (mode)
        {
            case 0:
            {
                pageReadNoteOrQuest.gameObject.SetActive(true);
                headerMain.text = playerNotes[num].name.text;
                noteMain.text = playerNotes[num].description.text;
                if (!playerNotes[num].read)
                    playerNotes[num].read = true;
                break;
            }
            case 1:
            {
                pageReadNoteOrQuest.gameObject.SetActive(true);
                Quest selectedQuest = QuestsSystem.Instance.activeQuests[num];
                headerMain.text = selectedQuest.name.text;
                noteMain.text = selectedQuest.description.text;
                if (selectedQuest.steps[selectedQuest.totalStep].name.text != "")
                    noteMain.text += "\n\n -<b>" + selectedQuest.steps[selectedQuest.totalStep].name + "</b>\n" +
                                     selectedQuest.steps[selectedQuest.totalStep].description;
                if (selectedQuest != QuestsSystem.Instance.totalQuest)
                {
                    //buttonChoiceActiveQuest.gameObject.SetActive(true);
                    //buttonChoiceActiveQuest.GetComponent<Button>().onClick.AddListener(delegate
                    //{
                    //QuestsSystem.Instance.ChoiceActiveQuest(selectedQuest.nameInGame);
                    //});
                }

                break;
            }
            case 2:
                Npc selectedNpc = Player.Instance.familiarNpc[num];
                pageReadHuman.gameObject.SetActive(true);
                headerHuman.text = selectedNpc.nameOfNpc.text;
                noteHuman.text = selectedNpc.description.text;
                iconHuman.sprite = selectedNpc.GetStyleIcon(NpcIcon.IconMood.Standart);
                relationshipTextHuman.text =
                    new LanguageSetting($"Отношения({selectedNpc.NpcController.npcEntity.relationshipWithPlayer})",
                        $"Relationships({selectedNpc.NpcController.npcEntity.relationshipWithPlayer})").text;
                sliderHuman.value = selectedNpc.NpcController.npcEntity.relationshipWithPlayer;
                break;
        }
    }

    public void CloseReadMenu()
    {
        pageReadHuman.gameObject.SetActive(false);
        pageReadNoteOrQuest.gameObject.SetActive(false);
        pageReadNoteOrQuest.gameObject.SetActive(false);
        pageReadMenu.gameObject.SetActive(false);
    }
}