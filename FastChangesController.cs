using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

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
    public string activateCutscene;
    public string activateQuest;

    [Header("-Locations")] public string moveToLocation;
    public string moveToLocationSpawn;
    public bool moveWithFade = true;

    [Header("-Locks")] public bool lockAllMenu;

    [Header("-Player")] public float editPlayerSpeed;
    public bool playerCanMove = true, playerCanMoveZ = true;
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
        Player.Instance.canMove = playerCanMove;
        Player.Instance.blockMoveZ = !playerCanMoveZ;
        CameraManager.Instance.SetVolumeProfile(newVolumeProfile);
        AudioManager.Instance.PlayMusic(setMusic);
        await Task.Delay(10);

        if (activateDialog != "")
            await DialogsManager.Instance.ActivateDialog(activateDialog);
        
        if (activateCutscene != "")
            await CutsceneManager.Instance.ActivateCutscene(activateCutscene);

        if (activateQuest != "")
            await QuestsManager.Instance.ActivateQuest(activateQuest);
        
        if (moveToLocation != "")
            await ManageLocation.Instance.ActivateLocation(moveToLocation, moveToLocationSpawn, moveWithFade);

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