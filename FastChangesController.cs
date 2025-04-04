using UnityEngine;

[System.Serializable]
public class FastChangesController
{
    [System.Serializable]
    public class ChangeRelationship
    {
        public Npc npc;
        public float valueChange;
    }

    public string activateDialog;

    [Header("-Locations")] public string moveToLocation;
    public string moveToLocationSpawn;
    public bool toLocationWithFade = true;

    [Header("-Locks")] public bool lockAllMenu;

    [Header("-DoInScripts")] public bool questStepNext;
    public float editSpeed;
    public string changeVisualPlayer = "";

    [Header("-AddAnything")] public string[] addItem;
    public string[] addQuests;
    public string[] addNote;
    public ChangeRelationship[] changeRelationships;

    public void ActivateChanges()
    {
        Player.Instance.changeSpeed = editSpeed;
        if (changeVisualPlayer != "")
            Player.Instance.ChangeStyle(changeVisualPlayer);
        if (moveToLocation != "")
            ManageLocation.Instance.ActivateLocation(moveToLocation,
                moveToLocationSpawn, toLocationWithFade);

        if (activateDialog != "")
            DialogsManager.Instance.ActivateDialog(activateDialog);

        if (questStepNext)
            QuestsSystem.Instance.NextStep();

        foreach (string quest in addQuests)
            QuestsSystem.Instance.ActivateQuest(quest, true);
        foreach (string item in addItem)
            InventoryManager.Instance.AddItem(item);
        foreach (string note in addNote)
            Notebook.Instance.AddNote(note);
        foreach (ChangeRelationship changer in changeRelationships)
        {
            changer.npc.relationshipWithPlayer += changer.valueChange;
            if (changer.valueChange != 0)
                NotifyManager.Instance.StartNewRelationshipNotify(changer.npc.nameOfNpc.text,
                    changer.valueChange);
        }
    }
}