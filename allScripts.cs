using UnityEngine;

public class AllScripts : MonoBehaviour
{
    [HideInInspector] public Player player;
    [HideInInspector] public DialogsManager dialogsManager;
    [HideInInspector] public Interactions interactions;
    [HideInInspector] public ManageLocation manageLocation;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Notebook notebook;
    [HideInInspector] public QuestsSystem questsSystem;
    [HideInInspector] public CutsceneManager cutsceneManager;
    [HideInInspector] public Main main;

    public void Initialize()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        dialogsManager = GameObject.Find("scripts").GetComponent<DialogsManager>();
        interactions = GameObject.Find("scripts").GetComponent<Interactions>();
        manageLocation = GameObject.Find("scripts").GetComponent<ManageLocation>();
        inventory = GameObject.Find("scripts").GetComponent<Inventory>();
        notebook = GameObject.Find("scripts").GetComponent<Notebook>();
        questsSystem = GameObject.Find("questMenu").GetComponent<QuestsSystem>();
        cutsceneManager = GameObject.Find("scripts").GetComponent<CutsceneManager>();
        main = GameObject.Find("scripts").GetComponent<Main>();
    }
}
