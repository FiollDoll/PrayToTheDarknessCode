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
    [HideInInspector] public PostProcessingController postProcessingController;
    [HideInInspector] public NotifyManager notifyManager;

    public void Initialize()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        dialogsManager = GetComponent<DialogsManager>();
        interactions = GetComponent<Interactions>();
        manageLocation = GetComponent<ManageLocation>();
        inventoryManager = GetComponent<InventoryManager>();
        notebook = GetComponent<Notebook>();
        questsSystem = GameObject.Find("questMenu").GetComponent<QuestsSystem>();
        cutsceneManager = GetComponent<CutsceneManager>();
        main = GetComponent<Main>();
        devTool = GetComponent<DevTool>();
        postProcessingController = GetComponent<PostProcessingController>();
        notifyManager = GetComponent<NotifyManager>();
    }
}
