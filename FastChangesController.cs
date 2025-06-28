using System.Collections.Generic;
using System.Threading.Tasks;
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

    public string controllerName;

    public string activateDialog;
    public string activateCutscene;
    public string activateQuest;

    [Header("-Locations")] public string moveToLocation;
    public string moveToLocationSpawn;

    [Header("-Locks")] public bool lockAllMenu;

    [Header("-Player")] public float editPlayerSpeed;
    public bool blockPlayerMove;
    public bool blockPlayerMoveZ;
    public VolumeProfile newVolumeProfile;

    [Header("-ChangeAnything")] public AudioClip setMusic;
    public List<string> addItem;
    public List<string> addNote;
    public List<ChangeRelationship> changeRelationships;
    public List<ChangeVisual> changeVisuals;
    public List<ChangeTransform> changeTransforms;

    public Task ActivateChanges() => SetChanges();

    private async Task SetChanges()
    {
        Player.Instance.changeSpeed = editPlayerSpeed;
        if (blockPlayerMove)
            Player.Instance.canMove = !Player.Instance.canMove;
        if (blockPlayerMoveZ)
            Player.Instance.blockMoveZ = !Player.Instance.blockMoveZ;
        if (newVolumeProfile)
            CameraManager.Instance.SetVolumeProfile(newVolumeProfile);
        if (setMusic)
            AudioManager.Instance.PlayMusic(setMusic);
        await Task.Delay(10);

        if (moveToLocation != "")
            await ManageLocation.Instance.ActivateLocation(moveToLocation, moveToLocationSpawn);

        if (activateCutscene != "")
            await CutsceneManager.Instance.ActivateCutscene(activateCutscene);

        if (activateDialog != "")
            await DialogsManager.Instance.ActivateDialog(activateDialog);

        if (activateQuest != "")
            await QuestsManager.Instance.ActivateQuest(activateQuest);

        foreach (string item in addItem)
            InventoryManager.Instance.AddItem(item);
        foreach (string note in addNote)
            NotesManager.Instance.AddNote(note);

        foreach (ChangeRelationship changer in changeRelationships)
        {
            if (!changer.npc)
                changer.npc = NpcManager.Instance.GetNpcByName(changer.npcName);
            changer.npc.relationshipWithPlayer += changer.valueChange;
            if (changer.valueChange != 0)
                NotifyManager.Instance.StartNewRelationshipNotify(changer.npc.nameOfNpc.text,
                    changer.valueChange);
        }

        await Task.Delay(10);

        foreach (ChangeVisual changer in changeVisuals)
            changer.npcToChange.NpcController.ChangeStyle(changer.newVisual);
        await Task.Delay(10);

        foreach (ChangeTransform changer in changeTransforms)
            changer.objToChange.transform.position = changer.newTransform.position;
    }
}