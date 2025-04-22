using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class FastChangesController
{
    [System.Serializable]
    public class ChangeRelationship
    {
        public Npc npc;
        [HideInInspector] public string npcName; // Для json
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

    [Header("-ChangeAnything")] public List<string> addItem;
    public List<string> addNote;
    public List<ChangeRelationship> changeRelationships;
    public List<ChangeVisual> changeVisuals;
    public List<ChangeTransform> changeTransforms;

    public void ActivateChanges()
    {
        Player.Instance.changeSpeed = editPlayerSpeed;
        if (blockPlayerMove)
            Player.Instance.canMove = !Player.Instance.canMove;
        if (blockPlayerMoveZ)
            Player.Instance.blockMoveZ = !Player.Instance.blockMoveZ;
        if (moveToLocation != "")
            ManageLocation.Instance.ActivateLocation(moveToLocation,
                moveToLocationSpawn, toLocationWithFade);

        if (activateDialog != "")
            DialogsManager.Instance.ActivateDialog(activateDialog);

        if (newVolumeProfile)
            CameraManager.Instance.SetVolumeProfile(newVolumeProfile);

        foreach (string item in addItem)
            InventoryManager.Instance.AddItem(item);
        foreach (string note in addNote)
            Notebook.Instance.AddNote(note);
        foreach (ChangeRelationship changer in changeRelationships)
        {
            if (!changer.npc)
                changer.npc = NpcManager.Instance.GetNpcByName(changer.npcName);
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