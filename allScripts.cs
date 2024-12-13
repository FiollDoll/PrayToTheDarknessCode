using UnityEngine;
using UnityEngine.Serialization;

public class AllScripts : MonoBehaviour
{
    public Player player;
    public DialogsManager dialogsManager;
    public Interactions interactions;
    public ManageLocation locations;
    public Inventory inventory;
    public Notebook notebook;
    [FormerlySerializedAs("quests")] public QuestsSystem questsSystem;
    public CutsceneManager cutsceneManager;
    public Main main;
}
