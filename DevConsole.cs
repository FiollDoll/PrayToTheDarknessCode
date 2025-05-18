using System.Collections;
using TMPro;
using UnityEngine;

public class DevConsole : MonoBehaviour
{
    public static DevConsole Instance { get; private set; }
    [SerializeField] private GameObject devMenu, presetsMenu, infoMenu;
    [SerializeField] private TextMeshProUGUI dialogText, locationText, playerText, otherText;

    private void Awake() => Instance = this;

    public void ManageDevMenu() => devMenu.SetActive(!devMenu.activeSelf);
    
    public void ActivatePresets()
    {
        infoMenu.SetActive(false);
        presetsMenu.SetActive(true);
    }

    public void ChangeLocation(string locationName) => StartCoroutine(ManageLocation.Instance.ActivateLocation(locationName));

    public void ChangeStyle(string newStyle) => Player.Instance.ChangeStyle(newStyle);
    
    public void ActivateInfo()
    {
        presetsMenu.SetActive(false);
        infoMenu.SetActive(true);
        StartCoroutine(UpdateInfo());
    }

    private IEnumerator UpdateInfo()
    {
        if (DialogsManager.Instance.activatedDialog != null)
            dialogText.text = "activated dialog: " + DialogsManager.Instance.activatedDialog.nameDialog + "\n" +
                              "selected branch: " + DialogsManager.Instance.selectedBranch.branchName + "\n" +
                              "Talker: " + DialogsManager.Instance.selectedStep.totalNpc.nameInWorld + "\n" +
                              "Mood: " + DialogsManager.Instance.selectedStep.iconMoodSelected + "\n" +
                              "TotalStep: " + DialogsManager.Instance.totalStep;
        else
            dialogText.text = "";
        if (ManageLocation.Instance.totalLocation != null)
        {
            locationText.text = "";
            locationText.text = "TotalLocation: " + ManageLocation.Instance.totalLocation.gameName + "\n";
            // Переделать
            foreach (NpcController npc in ManageLocation.Instance.npсs)
                locationText.text += npc.npcEntity?.nameInWorld + ", ";
        }
        else
            locationText.text = "";

        playerText.text = "SelectedStyle: " + Player.Instance.selectedStyle +"\n" +
                          "CanMove: " + Player.Instance.canMove + "\n" +
                          "CanMoveZ: " + !Player.Instance.blockMoveZ +"\n" +
                          "Position: " + Player.Instance.transform.position +"\n";

        yield return null;
        if (infoMenu.activeSelf)
            StartCoroutine(UpdateInfo());
    }
}