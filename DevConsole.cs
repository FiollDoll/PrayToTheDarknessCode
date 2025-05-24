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
        if (DialogsManager.Instance.SelectedDialog != null)
            dialogText.text = "Dialog: " + DialogsManager.Instance.SelectedDialog.nameDialog + "\n" +
                              "Branch: " + DialogsManager.Instance.SelectedBranch.branchName + "\n" +
                              "Talker: " + DialogsManager.Instance.SelectedStep.totalNpc.nameInWorld + "\n" +
                              "Mood: " + DialogsManager.Instance.SelectedStep.iconMoodSelected + "\n" +
                              "TotalStep: " + DialogsManager.Instance.TotalStep;
        else
            dialogText.text = "";
        if (ManageLocation.Instance.TotalLocation)
        {
            locationText.text = "";
            locationText.text = "TotalLocation: " + ManageLocation.Instance.TotalLocation.gameName + "\n";
            // Переделать
            //foreach (NpcController npc in ManageLocation.Instance.npсs)
                //locationText.text += npc.npcEntity?.nameInWorld + ", ";
        }
        else
            locationText.text = "";

        playerText.text = "Style: " + Player.Instance.selectedStyle +"\n" +
                          "CanMove: " + Player.Instance.canMove + "\n" +
                          "CanMoveZ: " + !Player.Instance.blockMoveZ +"\n" +
                          "Position: " + Player.Instance.transform.position +"\n";

        yield return null;
        if (infoMenu.activeSelf)
            StartCoroutine(UpdateInfo());
    }
}