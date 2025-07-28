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
    public bool changeLockInventory, changeLockHumansPage, changeLockPersonPage;

    [Header("-Player")] public float editPlayerSpeed;
    public bool playerCanMove = true, playerCanMoveZ = true;
    public VolumeProfile newVolumeProfile;

    [Header("-ChangeAnything")] public AudioClip setMusic;
    public List<Item> addItem;
    public List<Note> addNote;
    public List<ChangeRelationship> changeRelationships;
    public List<ChangeVisual> changeVisuals;

    public Task ActivateChanges() => SetChanges();

    private async Task SetChanges()
    {
        PlayerStats.ChangeSpeed = editPlayerSpeed;
        Player.Instance.canMove = playerCanMove;
        Player.Instance.blockMoveZ = !playerCanMoveZ;
        CameraManager.Instance.SetVolumeProfile(newVolumeProfile);
        AudioManager.Instance.PlayMusic(setMusic);

        // Если галочка стоит, то меняем значение на противоположное
        if (changeLockInventory)
            PlayerMenu.Instance.inventoryCan = !PlayerMenu.Instance.inventoryCan;
        if (changeLockHumansPage)
            PlayerMenu.Instance.humansCan = !PlayerMenu.Instance.humansCan;
        if (changeLockPersonPage)
            PlayerMenu.Instance.personCan = !PlayerMenu.Instance.personCan;

        // Выдаём хуйню
        await AddItems();
        await AddNotes();

        // Разные изменения
        await ChangeRelations();
        await ChangeVisuals();

        if (moveToLocation)
            await ManageLocation.Instance.ActivateLocation(moveToLocation.gameName, moveToLocationSpawn, moveWithFade);
        if (activateDialog)
            await DialogsManager.Instance.ActivateDialog(activateDialog.name);
        if (activateCutscene)
            await CutsceneManager.Instance.ActivateCutscene(activateCutscene.cutsceneName);
        if (activateQuest)
            await QuestsManager.Instance.ActivateQuest(activateQuest.questName);
    }

    private async Task AddItems()
    {
        foreach (Item item in addItem)
        {
            try
            {
                Inventory.Instance.AddItem(item.nameInGame);
            }
            catch (System.Exception ex)
            {
                Debug.Log("Item add error");
            }
        }
    }

    private async Task AddNotes()
    {
        foreach (Note note in addNote)
        {
            try
            {
                NotesManager.Instance.AddNote(note.gameName);
            }
            catch (System.Exception ex)
            {
                Debug.Log("Note add error");
            }
        }
    }

    private async Task ChangeRelations()
    {
        foreach (ChangeRelationship changer in changeRelationships)
        {
            try
            {
                changer.npc.relationshipWithPlayer += changer.valueChange;
                if (changer.valueChange != 0)
                    NotifyManager.Instance.StartNewRelationshipNotify(changer.npc.nameOfNpc.Text, changer.valueChange);
            }
            catch (System.Exception ex)
            {
                Debug.Log("ChangeRelation error");
            }
        }
    }

    private async Task ChangeVisuals()
    {
        foreach (ChangeVisual changer in changeVisuals)
        {
            try
            {
                changer.npcToChange.NpcController.ChangeStyle(changer.newVisual);
            }
            catch (System.Exception ex)
            {
                Debug.Log("VisualChange error");
            }
        }
    }
}

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