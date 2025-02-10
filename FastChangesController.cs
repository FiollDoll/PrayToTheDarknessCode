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

    public void ActivateChanges(AllScripts scripts)
    {
        scripts.player.changeSpeed = editSpeed;
        if (changeVisualPlayer != "")
            scripts.player.ChangeStyle(changeVisualPlayer);
        if (moveToLocation != "")
            scripts.manageLocation.ActivateLocation(moveToLocation,
                moveToLocationSpawn, toLocationWithFade);

        if (activateDialog != "")
            scripts.dialogsManager.ActivateDialog(activateDialog);

        if (questStepNext)
            scripts.questsSystem.NextStep();

        foreach (string quest in addQuests)
            scripts.questsSystem.ActivateQuest(quest, true);
        foreach (string item in addItem)
            scripts.inventoryManager.AddItem(item);
        foreach (string note in addNote)
            scripts.notebook.AddNote(note);
        foreach (ChangeRelationship changer in changeRelationships)
        {
            changer.npc.relationshipWithPlayer += changer.valueChange;
            if (changer.valueChange != 0)
                scripts.notifyManager.StartNewRelationshipNotify(changer.npc.nameOfNpc.text,
                    changer.valueChange);
        }
    }
}