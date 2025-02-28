using UnityEngine;
using Newtonsoft.Json;

public class SaveAndLoadManager : MonoBehaviour
{
    [SerializeField] private GameSave gameSave;
    [SerializeField] private AllScripts scripts;

    private void Start()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        gameSave = new GameSave();
        gameSave.playerStyle = scripts.player.selectedStyle;
        gameSave.x = scripts.player.transform.position.x;
        gameSave.y = scripts.player.transform.position.y;
        gameSave.z = scripts.player.transform.position.z;
        gameSave.playerLocationName = scripts.manageLocation.totalLocation.gameName;
        gameSave.playerNotes = scripts.notebook.playerNotes;
        gameSave.playerItems = scripts.inventoryManager.inventory.playerItems;
        gameSave.familiarNpc = scripts.player.familiarNpc;
        gameSave.playerQuests = scripts.questsSystem.activeQuests;
        string json = JsonConvert.SerializeObject(gameSave, Formatting.Indented);
        Debug.Log(json);
        // TODO: Сохранение
    }

    public void LoadGame()
    {
        // TODO: Загрузка
        string json = "";
        gameSave = JsonConvert.DeserializeObject<GameSave>(json);
        scripts.player.ChangeStyle(gameSave.playerStyle);
        scripts.player.transform.position = new Vector3(gameSave.x, gameSave.y, gameSave.z);
        scripts.manageLocation.totalLocation = scripts.manageLocation.GetLocation(gameSave.playerLocationName);
        scripts.notebook.playerNotes = gameSave.playerNotes;
        scripts.inventoryManager.inventory.playerItems = gameSave.playerItems;
        scripts.player.familiarNpc = gameSave.familiarNpc;
        scripts.questsSystem.activeQuests = gameSave.playerQuests;
    }
}