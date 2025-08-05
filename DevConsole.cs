using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevConsole : MonoBehaviour
{
    [SerializeField] private GameObject devMenu, presetsMenu, infoMenu;
    [SerializeField] private TextMeshProUGUI dialogText, locationText, playerText, otherText;

    public void OnManageDevConsole(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
            devMenu.SetActive(!devMenu.activeSelf);
    }

    public void ActivatePresets()
    {
        infoMenu.SetActive(false);
        presetsMenu.SetActive(true);
    }

    public void SkipDialog()
    {
        DialogsManager.Instance.DialogCLose();
    }

    public async void ChangeLocation(string locationName) =>
        await ManageLocation.Instance.ActivateLocation(locationName);

    public void ChangeStyle(string newStyle) => Player.Instance.ChangeStyle(newStyle);

    public void ChangeChapter(string chapter) =>
        ChapterManager.Instance.StartLoadChapter(ChapterManager.Instance.GetChapterByName(chapter));

    public void UnlockAllRelationship()
    {
        foreach (Npc npc in NpcManager.Instance.AllNpc)
        {
            if (npc.canMeet)
            {
                PlayerStats.FamiliarNpc.Add(npc);
                NpcManager.Instance.npcTempInfo[npc].meetWithPlayer = true;
            }
        }
    }

    public void UnlockAllNotes()
    {
        foreach (Note note in NotesManager.Instance.Notes)
            NotesManager.Instance.AddNote(note.gameName);
    }

    public void UnlockMove()
    {
        Player.Instance.blockMoveZ = false;
        Player.Instance.canMove = true;
    }

    public async void ActivateInfo()
    {
        presetsMenu.SetActive(false);
        infoMenu.SetActive(true);
        await UpdateInfo();
    }

    private async Task UpdateInfo()
    {
        if (devMenu.activeSelf)
        {
            if (DialogUI.Instance.story)
                dialogText.text = "Dialog: " + DialogUI.Instance.story.name + "\n" +
                                  "Dialog mode: " + DialogUI.Instance.story.GetFirstNode().styleOfDialog.ToString() + "\n" +
                                  "Talker: " + DialogUI.Instance.currentDialogStepNode.character.nameInWorld + "\n" +
                                  "Mood: " + DialogUI.Instance.currentDialogStepNode.mood;
            else
                dialogText.text = "";
            if (ManageLocation.Instance.TotalLocation)
            {
                locationText.text = "";
                locationText.text = "TotalLocation: " + ManageLocation.Instance.TotalLocation.gameName + "\n";
                foreach (NpcController npc in ManageLocation.Instance.NpcAtTotalLocation)
                    locationText.text += npc.gameObject.name + ", ";
            }
            else
                locationText.text = "";

            playerText.text = "Style: " + Player.Instance.SelectedStyle + "Person: " + Player.Instance.selectedPerson.npcEntity.nameOfNpc.Text +"\n" +
                              "CanMove: " + Player.Instance.canMove + "| Z " + !Player.Instance.blockMoveZ + "\n" +
                              "Position: " + Player.Instance.transform.position + "\n";
        }

        await Task.Delay(100);
        if (infoMenu.activeSelf)
            await UpdateInfo();
    }
}