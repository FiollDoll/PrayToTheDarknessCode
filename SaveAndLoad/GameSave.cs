using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Структурный класс для хранения в json
/// </summary>
[System.Serializable]
public class GameSave
{
    public string playerStyle;
    public float x, y, z;
    public string playerLocationName;
    public List<Note> playerNotes;
    public List<Item> playerItems;
    public List<Npc> familiarNpc;
}