using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class SaveAndLoadManager
{
    public static SaveAndLoadManager Instance { get; private set; }
    private GameSave gameSave;

    public async Task Initialize()
    {
        Instance = this;
        await LoadGame();
    }
    
    public Task SaveGame()
    {
        gameSave = new GameSave();
        gameSave.playerStyle = Player.Instance.SelectedStyle;
        gameSave.x = Player.Instance.transform.position.x;
        gameSave.y = Player.Instance.transform.position.y;
        gameSave.z = Player.Instance.transform.position.z;
        gameSave.playerLocationName = ManageLocation.Instance.TotalLocation.gameName;
        //gameSave.playerNotes = Notebook.Instance.playerNotes;
        //gameSave.playerItems = InventoryManager.Instance.inventory.playerItems;
        gameSave.familiarNpc = PlayerStats.FamiliarNpc;
        //string json = JsonConvert.SerializeObject(gameSave, Formatting.Indented);
        return Task.CompletedTask;
        // TODO: Сохранение
    }

    private Task LoadGame()
    {
        // TODO: Загрузка
        string json = "";
        gameSave = JsonConvert.DeserializeObject<GameSave>(json);
        
        if (gameSave != null)
        {
            ManageLocation.Instance.TotalLocation = ManageLocation.Instance.GetLocation(gameSave.playerLocationName);
            NotesManager.Instance.PlayerNotes = gameSave.playerNotes;
            Inventory.Instance.playerItems = gameSave.playerItems;
            Player.Instance.ChangeStyle(gameSave.playerStyle);
            Player.Instance.transform.position = new Vector3(gameSave.x, gameSave.y, gameSave.z);
            PlayerStats.FamiliarNpc = gameSave.familiarNpc;
        }

        return Task.CompletedTask;
    }
}