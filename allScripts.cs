using UnityEngine;

public class AllScripts : MonoBehaviour
{
    [HideInInspector] public Player player;
    [HideInInspector] public DialogsManager dialogsManager;
    [HideInInspector] public Interactions interactions;
    [HideInInspector] public ManageLocation manageLocation;
    [HideInInspector] public InventoryManager inventoryManager;
    [HideInInspector] public Notebook notebook;
    [HideInInspector] public QuestsSystem questsSystem;
    [HideInInspector] public CutsceneManager cutsceneManager;
    [HideInInspector] public Main main;
    [HideInInspector] public DevTool devTool;

    public void Initialize()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        dialogsManager = GameObject.Find("scripts").GetComponent<DialogsManager>();
        interactions = GameObject.Find("scripts").GetComponent<Interactions>();
        manageLocation = GameObject.Find("scripts").GetComponent<ManageLocation>();
        inventoryManager = GameObject.Find("scripts").GetComponent<InventoryManager>();
        notebook = GameObject.Find("scripts").GetComponent<Notebook>();
        questsSystem = GameObject.Find("questMenu").GetComponent<QuestsSystem>();
        cutsceneManager = GameObject.Find("scripts").GetComponent<CutsceneManager>();
        main = GameObject.Find("scripts").GetComponent<Main>();
        devTool = GameObject.Find("devTools").GetComponent<DevTool>();
    }
}
