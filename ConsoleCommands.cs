using UnityEngine;
using AtomicConsole;
using AtomicConsole.debug;

public class ConsoleCommands : MonoBehaviour
{

    [AtomicCommand(name: "ActivateLocation")]
    public void ActivateLocation(string location) => ManageLocation.Instance.ActivateLocation(location);

    [AtomicCommand(name: "ListLocations")]
    public void GetListLocations()
    {
        foreach (Location location in ManageLocation.Instance.locations)
            AtomicDebug.Command(location.gameName);
    }

    [AtomicCommand(name: "ChangePlayerStyle")]
    public void ChangePlayerStyle(string style) => Player.Instance.ChangeStyle(style);
    
    [AtomicCommand(name: "NewQuest")]
    public void ActivateQuest(string questName) => QuestsSystem.Instance.ActivateQuest(questName);
    
    [AtomicCommand(name: "NewQuest")]
    public void ActivateStepQuest(int step)
    {
        
    }
    
    [AtomicCommand(name: "AddItem")]
    public void AddItem(string item) => InventoryManager.Instance.AddItem(item);
}