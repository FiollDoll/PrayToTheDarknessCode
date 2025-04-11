using System;
using UnityEngine;
using Newtonsoft.Json;

public class SaveAndLoadManager : MonoBehaviour
{
    public static SaveAndLoadManager Instance { get; private set; }
    [SerializeField] private GameSave gameSave;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //LoadGame();
    }

    public void SaveGame()
    {
        gameSave = new GameSave();
        gameSave.playerStyle = Player.Instance.selectedStyle;
        gameSave.x = Player.Instance.transform.position.x;
        gameSave.y = Player.Instance.transform.position.y;
        gameSave.z = Player.Instance.transform.position.z;
        gameSave.playerLocationName = ManageLocation.Instance.totalLocation.gameName;
        gameSave.playerNotes = Notebook.Instance.playerNotes;
        gameSave.playerItems = InventoryManager.Instance.inventory.playerItems;
        gameSave.familiarNpc = Player.Instance.familiarNpc;
        string json = JsonConvert.SerializeObject(gameSave, Formatting.Indented);
        Debug.Log(json);
        // TODO: Сохранение
    }

    public void LoadGame()
    {
        // TODO: Загрузка
        string json = "";
        gameSave = JsonConvert.DeserializeObject<GameSave>(json);
        Player.Instance.ChangeStyle(gameSave.playerStyle);
        Player.Instance.transform.position = new Vector3(gameSave.x, gameSave.y, gameSave.z);
        ManageLocation.Instance.totalLocation = ManageLocation.Instance.GetLocation(gameSave.playerLocationName);
        Notebook.Instance.playerNotes = gameSave.playerNotes;
        InventoryManager.Instance.inventory.playerItems = gameSave.playerItems;
        Player.Instance.familiarNpc = gameSave.familiarNpc;
    }
}