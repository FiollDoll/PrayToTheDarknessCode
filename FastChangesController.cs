using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class FastChangesController
{
    [System.Serializable]
    public class ChangeRelationship
    {
        public Npc npc;
        public float valueChange;
    }

    [System.Serializable]
    public class ChangeVisual
    {
        public Npc npcToChange;
        public string newVisual;
    }

    [System.Serializable]
    public class ChangeTransform
    {
        public GameObject objToChange;
        public Transform newTransform;
    }

    public string activateDialog;

    [Header("-Locations")] public string moveToLocation;
    public string moveToLocationSpawn;
    public bool toLocationWithFade = true;

    [Header("-Locks")] public bool lockAllMenu;

    [Header("-Player")] public float editPlayerSpeed;
    public bool blockPlayerMove;
    public bool blockPlayerMoveZ;
    public VolumeProfile newVolumeProfile;

    [Header("-ChangeAnything")] public string[] addItem;
    public string[] addQuests;
    public string[] addNote;
    public ChangeRelationship[] changeRelationships;
    public ChangeVisual[] changeVisuals;
    public ChangeTransform[] changeTransforms;

    public void ActivateChanges()
    {
        Player.Instance.changeSpeed = editPlayerSpeed;
        Player.Instance.canMove = !blockPlayerMove;
        Player.Instance.blockMoveZ = blockPlayerMoveZ;
        if (moveToLocation != "")
            ManageLocation.Instance.ActivateLocation(moveToLocation,
                moveToLocationSpawn, toLocationWithFade);

        if (activateDialog != "")
            DialogsManager.Instance.ActivateDialog(activateDialog);
        
        if (newVolumeProfile != null)
            CameraManager.Instance.SetVolumeProfile(newVolumeProfile);
        
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

        foreach (ChangeVisual changer in changeVisuals)
            changer.npcToChange.NpcController.ChangeStyle(changer.newVisual);
        foreach (ChangeTransform changer in changeTransforms)
            changer.objToChange.transform.position = changer.newTransform.position;
    }
}