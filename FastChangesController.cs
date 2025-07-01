using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "FastChangesController")]
public class FastChangesController : ScriptableObject
{
    public Dialog activateDialog;
    public Cutscene activateCutscene;
    public Quest activateQuest;

    [Header("-Locations")] public Location moveToLocation;
    public string moveToLocationSpawn;
    public bool moveWithFade = true;

    [Header("-Locks")] public bool lockAllMenu;

    [Header("-Player")] public float editPlayerSpeed;
    public bool playerCanMove = true, playerCanMoveZ = true;
    public VolumeProfile newVolumeProfile;

    [Header("-ChangeAnything")] public AudioClip setMusic;
    public List<Item> addItem;
    public List<Note> addNote;
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

        if (activateDialog)
            await DialogsManager.Instance.ActivateDialog(activateDialog.name);
        if (activateCutscene)
            await CutsceneManager.Instance.ActivateCutscene(activateCutscene.cutsceneName);
        if (activateQuest)
            await QuestsManager.Instance.ActivateQuest(activateQuest.questName);
        if (moveToLocation)
            await ManageLocation.Instance.ActivateLocation(moveToLocation.gameName, moveToLocationSpawn, moveWithFade);

        foreach (Item item in addItem)
            InventoryManager.Instance.AddItem(item.nameInGame);
        foreach (Note note in addNote)
            NotesManager.Instance.AddNote(note.gameName);

        foreach (ChangeRelationship changer in changeRelationships)
        {
            if (!changer.npc)
                changer.npc = NpcManager.Instance.GetNpcByName(changer.npcName);
            changer.npc.relationshipWithPlayer += changer.valueChange;
            if (changer.valueChange != 0)
                NotifyManager.Instance.StartNewRelationshipNotify(changer.npc.nameOfNpc.Text,
                    changer.valueChange);
        }

        foreach (ChangeVisual changer in changeVisuals)
            changer.npcToChange.NpcController.ChangeStyle(changer.newVisual);

        foreach (ChangeTransform changer in changeTransforms)
            changer.objToChange.transform.position = changer.newTransform.position;
    }
}

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