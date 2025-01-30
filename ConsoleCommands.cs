using UnityEngine;
using AtomicConsole;
using AtomicConsole.debug;

public class ConsoleCommands : MonoBehaviour
{
    [SerializeField] private AllScripts scripts;

    [AtomicCommand(name: "ActivateLocation")]
    public void ActivateLocation(string location) => scripts.manageLocation.ActivateLocation(location);

    [AtomicCommand(name: "ListLocations")]
    public void GetListLocations()
    {
        foreach (Location location in scripts.manageLocation.locations)
            AtomicDebug.Command(location.gameName);
    }

    [AtomicCommand(name: "ChangePlayerStyle")]
    public void ChangePlayerStyle(string style) => scripts.player.ChangeStyle(style);
    
    [AtomicCommand(name: "NewQuest")]
    public void ActivateQuest(string questName) => scripts.questsSystem.ActivateQuest(questName);
    
    [AtomicCommand(name: "NewQuest")]
    public void ActivateStepQuest(int step)
    {
        
    }
    
    [AtomicCommand(name: "AddItem")]
    public void AddItem(string item) => scripts.inventoryManager.AddItem(item);
}