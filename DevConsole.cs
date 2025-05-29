using System.Collections;
using TMPro;
using UnityEngine;

public class DevConsole : MonoBehaviour
{
    public static DevConsole Instance { get; private set; }
    public bool devMode;
    [SerializeField] private GameObject devMenu, presetsMenu, infoMenu;
    [SerializeField] private TextMeshProUGUI dialogText, locationText, playerText, otherText;

    private void Awake() => Instance = this;

    public void ManageDevMenu()
    {
        if (devMode)
            devMenu.SetActive(!devMenu.activeSelf);
    }

    public void ActivatePresets()
    {
        infoMenu.SetActive(false);
        presetsMenu.SetActive(true);
    }

    public void ChangeLocation(string locationName) =>
        StartCoroutine(ManageLocation.Instance.ActivateLocation(locationName));

    public void ChangeStyle(string newStyle) => Player.Instance.ChangeStyle(newStyle);

    public void ChangeChapter(string chapter) =>
        ChapterManager.Instance.StartLoadChapter(ChapterManager.Instance.GetChapterByName(chapter));

    public void UnlockAllRelationship()
    {
        foreach (Npc npc in NpcManager.Instance.AllNpc)
        {
            if (npc.canMeet)
                Player.Instance.familiarNpc.Add(npc);
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
    
    public void ActivateInfo()
    {
        presetsMenu.SetActive(false);
        infoMenu.SetActive(true);
        StartCoroutine(UpdateInfo());
    }

    private IEnumerator UpdateInfo()
    {
        if (DialogUI.Instance.story)
            dialogText.text = "Dialog: " + DialogUI.Instance.story.name + "\n" +
                              "Node id: " + DialogUI.Instance.currentNode.guid + "\n" +
                              "Talker: " + DialogUI.Instance.currentNode.characterName + "\n" +
                              "Mood: " + DialogUI.Instance.currentNode.mood;
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

        playerText.text = "Style: " + Player.Instance.selectedStyle + "\n" +
                          "CanMove: " + Player.Instance.canMove + "\n" +
                          "CanMoveZ: " + !Player.Instance.blockMoveZ + "\n" +
                          "Position: " + Player.Instance.transform.position + "\n";

        yield return null;
        if (infoMenu.activeSelf)
            StartCoroutine(UpdateInfo());
    }
}