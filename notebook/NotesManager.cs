using System.Collections.Generic;
using UnityEngine;

public class NotesManager
{
    public static NotesManager Instance { get; private set; }
    public List<Note> PlayerNotes = new List<Note>();

    public Note[] Notes;
    private readonly Dictionary<string, Note> _gameNotesDict = new Dictionary<string, Note>();

    public void Initialize()
    {
        Instance = this;
        Notes = Resources.LoadAll<Note>("Notes/");
        foreach (Note note in Notes)
            _gameNotesDict.TryAdd(note.gameName, note);
    }

    /// <summary>
    /// Получить записку по имени
    /// </summary>
    /// <param name="nameNote"></param>
    /// <returns></returns>
    private Note GetNote(string nameNote) => _gameNotesDict.GetValueOrDefault(nameNote);
    
    public void AddNote(string nameNote)
    {
        Note newNote = GetNote(nameNote);
        if (!newNote) return; // Если пустой

        PlayerNotes.Add(newNote);
        NotifyManager.Instance.StartNewNoteNotify(newNote.noteName.text);
    }
}