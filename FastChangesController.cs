using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "FastChangesController")]
public class FastChangesController : ScriptableObject
{
    public Dialog activateDialog;
    public Quest activateQuest;

    [Header("-Locations")] public Location moveToLocation;
    public string moveToLocationSpawn;
    public bool moveWithFade = true;

    [Header("-Locks")] public bool lockAllMenu;
    public bool changeLockInventory, changeLockHumansPage, changeLockPersonPage;

    [Header("-Player")] public float editPlayerSpeed;
    public bool playerCanMove = true, playerCanMoveZ = true;
    public VolumeProfile newVolumeProfile;

    [Header("-Npc")]
    public ChangeTempInfoNpc[] changeTempInfoNpc = new ChangeTempInfoNpc[0];
    public TeleportNpc[] teleportNpc = new TeleportNpc[0];
    public MoveToPlayer[] changeMoveToPlayer = new MoveToPlayer[0];

    [Header("-ChangeAnything")] public AudioClip setMusic;
    public List<Item> addItem;
    public List<Note> addNote;
    public Npc[] addFamiliar = new Npc[0];
    public ChangeVisual[] changeVisuals = new ChangeVisual[0];

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
        await AddFamiliar();
        await ChangeNpcTemp();
        await ChangeNpcPosition();
        await ChangeVisuals();
        await ChangeMoveToPlayer();

        if (moveToLocation)
            await ManageLocation.Instance.ActivateLocation(moveToLocation.gameName, moveToLocationSpawn, moveWithFade);
        if (activateDialog)
            await DialogsManager.Instance.ActivateDialog(activateDialog.name);
        if (activateQuest)
            await QuestsManager.Instance.ActivateQuest(activateQuest.questName);
    }

    private async Task AddFamiliar()
    {
        foreach (Npc totalNpc in addFamiliar)
        {
            try
            {
                if (!totalNpc.canMeet) continue;
                if (PlayerStats.FamiliarNpc.Contains(totalNpc)) continue;
                PlayerStats.FamiliarNpc.Add(totalNpc);
            }
            catch (System.Exception ex)
            {
                Debug.Log("Pisdec familiar ne dobavilsya");
            }
        }
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

    private async Task ChangeNpcPosition()
    {
        foreach (TeleportNpc tp in teleportNpc)
        {
            try
            {
                Transform target = GameObject.Find(tp.teleportName)?.transform;
                if (target != null && tp.npc.IHumanable is NpcController npcController)
                    npcController.gameObject.transform.position = target.position;

            }
            catch (System.Exception ex)
            {
                Debug.Log("Npc teleport error");
            }
        }
    }

    private async Task ChangeNpcTemp()
    {
        foreach (ChangeTempInfoNpc changer in changeTempInfoNpc)
        {
            try
            {
                NpcTempInfo npcTemp = NpcManager.Instance.npcTempInfo[changer.npc];
                npcTemp.relationshipWithPlayer += changer.relationshipChange;
                if (changer.relationshipChange != 0)
                    NotifyManager.Instance.StartNewRelationshipNotify(changer.npc.nameOfNpc.Text, changer.relationshipChange);
                npcTemp.knowAge = changer.unlockAge;
                npcTemp.knowCharacter = changer.unlockCharacter;
                npcTemp.knowHobby = changer.unlockHobby;
                npcTemp.knowHouse = changer.unlockHouse;
                npcTemp.knowProfession = changer.unlockProfession;
            }
            catch (System.Exception ex)
            {
                Debug.Log("ChangeNpcTemp error");
            }
        }
    }

    private async Task ChangeVisuals()
    {
        foreach (ChangeVisual changer in changeVisuals)
        {
            try
            {
                changer.npcToChange.IHumanable.ChangeStyle(changer.newVisual);
            }
            catch (System.Exception ex)
            {
                Debug.Log("VisualChange error");
            }
        }
    }

    private async Task ChangeMoveToPlayer()
    {
        foreach (MoveToPlayer changer in changeMoveToPlayer)
        {
            try
            {
                if (changer.npc.IHumanable is NpcController npcController)
                    npcController.moveToPlayer = changer.newState;
            }
            catch (System.Exception ex)
            {
                Debug.Log("MoveToPlayer error");
            }
        }
    }
}

[System.Serializable]
public class TeleportNpc
{
    public Npc npc;
    public string teleportName;
}

[System.Serializable]
public class ChangeTempInfoNpc
{
    public Npc npc;
    public float relationshipChange;
    public bool unlockProfession, unlockHobby, unlockAge, unlockCharacter, unlockHouse;
}

[System.Serializable]
public class ChangeVisual
{
    public Npc npcToChange;
    public string newVisual;
}

[System.Serializable]
public class MoveToPlayer
{
    public Npc npc;
    public bool newState;
}