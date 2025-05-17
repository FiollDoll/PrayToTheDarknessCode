using System.Collections;
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
    
    public void ActivateChanges() => CoroutineContainer.Instance.StartCoroutine(SetChanges());

    private IEnumerator SetChanges()
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
        yield return null;
        
        if (moveToLocation != "")
        {
            Coroutine coroutine = CoroutineContainer.Instance.StartCoroutine(ManageLocation.Instance.ActivateLocation(
                moveToLocation,
                moveToLocationSpawn));
            yield return coroutine;
        }

        if (activateCutscene != "")
        {
            CutsceneManager.Instance.ActivateCutscene(activateCutscene);
            yield return null;
        }

        if (activateDialog != "")
        {
            DialogsManager.Instance.ActivateDialog(activateDialog);
            yield return null;
        }
        if (activateQuest != "")
            QuestsManager.Instance.ActivateQuest(activateQuest);
        yield return null;
        
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
        yield return null;

        foreach (ChangeVisual changer in changeVisuals)
            changer.npcToChange.NpcController.ChangeStyle(changer.newVisual);
        yield return null;
        
        foreach (ChangeTransform changer in changeTransforms)
            changer.objToChange.transform.position = changer.newTransform.position;
        yield return null;
    }
}