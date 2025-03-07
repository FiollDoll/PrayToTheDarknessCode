using System;
using UnityEngine;
using Newtonsoft.Json;

public class SaveAndLoadManager : MonoBehaviour
{
    public static SaveAndLoadManager singleton { get; private set; }
    [SerializeField] private GameSave gameSave;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        gameSave = new GameSave();
        gameSave.playerStyle = Player.singleton.selectedStyle;
        gameSave.x = Player.singleton.transform.position.x;
        gameSave.y = Player.singleton.transform.position.y;
        gameSave.z = Player.singleton.transform.position.z;
        gameSave.playerLocationName = ManageLocation.singleton.totalLocation.gameName;
        gameSave.playerNotes = Notebook.singleton.playerNotes;
        gameSave.playerItems = InventoryManager.singleton.inventory.playerItems;
        gameSave.familiarNpc = Player.singleton.familiarNpc;
        gameSave.playerQuests = QuestsSystem.singleton.activeQuests;
        string json = JsonConvert.SerializeObject(gameSave, Formatting.Indented);
        Debug.Log(json);
        // TODO: Сохранение
    }

    public void LoadGame()
    {
        // TODO: Загрузка
        string json = "";
        gameSave = JsonConvert.DeserializeObject<GameSave>(json);
        Player.singleton.ChangeStyle(gameSave.playerStyle);
        Player.singleton.transform.position = new Vector3(gameSave.x, gameSave.y, gameSave.z);
        ManageLocation.singleton.totalLocation = ManageLocation.singleton.GetLocation(gameSave.playerLocationName);
        Notebook.singleton.playerNotes = gameSave.playerNotes;
        InventoryManager.singleton.inventory.playerItems = gameSave.playerItems;
        Player.singleton.familiarNpc = gameSave.familiarNpc;
        QuestsSystem.singleton.activeQuests = gameSave.playerQuests;
    }
}