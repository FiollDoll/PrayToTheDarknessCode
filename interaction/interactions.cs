using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class interactions : MonoBehaviour
{
    private Dictionary<Collider2D, string> enteredColliders = new Dictionary<Collider2D, string>();
    public GameObject floorChangeMenu;
    [SerializeField] private TextMeshProUGUI interLabelText;
    [SerializeField] private allScripts scripts;
    private string totalColliderName, totalColliderMode, spawnName;
    private int selectedColliderId;
    public extraInter selectedEI = null;

    private void AddEnteredCollider(Collider2D other, string label)
    {
        selectedColliderId = 0;
        if (!enteredColliders.ContainsKey(other))
        {
            enteredColliders.Add(other, label);
            UpdateIntersMenu();
        }
    }

    private void UpdateIntersMenu()
    {
        interLabelText.text = "";
        int idx = 0;
        foreach (string label in enteredColliders.Values)
        {
            if (selectedColliderId == idx)
                interLabelText.text += ("> " + label + "\n");
            else
                interLabelText.text += (label + "\n");
            idx++;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "dialog":
                scripts.dialogsManager.ActivateDialog(other.gameObject.name);
                break;
            case "location":
                totalColliderName = other.gameObject.name;
                totalColliderMode = "location";
                selectedEI = other.gameObject.GetComponent<extraInteraction>().interactions[0];
                spawnName = selectedEI.moveToSpawn;

                if (!scripts.locations.GetLocation(totalColliderName).locked)
                {
                    if (scripts.locations.GetLocation(totalColliderName).autoEnter && scripts.locations.totalLocation.autoEnter)
                        scripts.locations.ActivateLocation(totalColliderName, spawnName);
                    else
                        AddEnteredCollider(other, selectedEI.interLabel);
                }
                break;
            case "cutscene":
            case "interact":
            case "item":
                if (other.gameObject.GetComponent<extraInteraction>() != null)
                {
                    extraInteraction EI = other.gameObject.GetComponent<extraInteraction>();
                    for (int i = 0; i < EI.interactions.Length; i++)
                    {
                        extraInter totalEI = EI.interactions[i];
                        if (scripts.quests.totalQuest != null)
                        {
                            if (totalEI.nameQuestRequired != scripts.quests.totalQuest.nameInGame)
                            {
                                if (totalEI.nameQuestRequired != "")
                                    continue;
                            }

                            if (totalEI.stageInter != scripts.quests.totalQuest.totalStep && totalEI.nameQuestRequired != "")
                                continue;
                        }
                        selectedEI = EI.interactions[i];
                        other.gameObject.name = totalEI.interName; // Сделано плохо
                        AddEnteredCollider(other, selectedEI.interLabel);
                        break;
                    }
                    if (selectedEI == null)
                        return;
                }
                totalColliderName = other.gameObject.name;
                totalColliderMode = other.gameObject.tag;
                break;
            case "cutsceneAuto":
                scripts.cutsceneManager.ActivateCutscene(other.gameObject.name);
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "cutscene":
            case "cutsceneAuto":
            case "interact":
            case "item":
            case "location":
                if (enteredColliders.ContainsKey(other))
                {
                    enteredColliders.Remove(other);
                    UpdateIntersMenu();
                }
                break;
        }
        if (floorChangeMenu.gameObject.activeSelf)
            floorChangeMenu.gameObject.SetActive(false);
        selectedEI = null;
        totalColliderName = "";
        totalColliderMode = "";
        spawnName = "";
    }

    private void ChoiceInter(int mode)
    {
        if (enteredColliders.Keys.Count != selectedColliderId + mode && selectedColliderId + mode >= 0)
        {
            selectedColliderId += mode;
            UpdateIntersMenu();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChoiceInter(1);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ChoiceInter(-1);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (enteredColliders.Keys.Count > 0)
            {
                totalColliderName = enteredColliders.ElementAt(selectedColliderId).Key.gameObject.name;
                if (totalColliderMode != "location")
                {
                    if (enteredColliders.ElementAt(selectedColliderId).Key.gameObject.GetComponent<extraInteraction>() != null)
                    {
                        if (selectedEI == null)
                            return;
                    }
                }
                if (selectedEI != null)
                {
                    scripts.inventory.AddItem(selectedEI.itemNameAdd);
                    if (selectedEI.darkAfterUse)
                        scripts.main.ActivateNoVision(1f);
                    if (selectedEI.NextStep && selectedEI.stageInter == scripts.quests.totalQuest.totalStep)
                        scripts.quests.NextStep();
                    if (selectedEI.activateCutscene)
                        scripts.cutsceneManager.ActivateCutscene(totalColliderName);
                    if (selectedEI.swapPlayerVisual)
                        scripts.player.ChangeVisual(selectedEI.playerVisual);
                    if (selectedEI.destroyAfterInter)
                        Destroy(GameObject.Find(totalColliderName));
                    if (selectedEI.moveToSpawn != "")
                        scripts.locations.ActivateLocation(totalColliderName, selectedEI.moveToSpawn);
                }
                switch (totalColliderMode)
                {
                    case "item":
                        scripts.inventory.AddItem(totalColliderName);
                        Destroy(GameObject.Find(totalColliderName));
                        UpdateIntersMenu();
                        break;
                    case "location":
                        if (!scripts.locations.GetLocation(totalColliderName).locked)
                            scripts.locations.ActivateLocation(totalColliderName, selectedEI.moveToSpawn);
                        break;
                    default:
                        if (totalColliderName == "floorChange" && (!scripts.main.CheckAnyMenuOpen() || floorChangeMenu.gameObject.activeSelf))
                            floorChangeMenu.gameObject.SetActive(!floorChangeMenu.gameObject.activeSelf);
                        else
                        {
                            if (!scripts.dialogsManager.dialogMenu.activeSelf)
                                scripts.dialogsManager.ActivateDialog(totalColliderName);
                        }
                        break;
                }
            }
        }
            if (Input.GetKeyDown(KeyCode.I))
                scripts.inventory.ManageInventoryPanel();
            if (Input.GetKeyDown(KeyCode.Tab))
                scripts.notebook.ManageNotePanel();
    }
}
