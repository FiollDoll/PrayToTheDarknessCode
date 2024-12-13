using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Interactions : MonoBehaviour
{
    public bool lockInter;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private Transform rayStart;
    private RaycastHit2D[] enteredColliders = new RaycastHit2D[0];
    public GameObject floorChangeMenu;
    [SerializeField] private int selectedColliderId;
    [SerializeField] private TextMeshProUGUI interLabelText;
    [SerializeField] private AllScripts scripts;
    private string totalColliderName, totalColliderMode, spawnName;
    public extraInter selectedEI = null;

    private void UpdateIntersMenu(int idx, string label)
    {
        if (selectedColliderId == idx)
            interLabelText.text += ("> " + label + "\n");
        else
            interLabelText.text += (label + "\n");
    }

    private void ChoiceInter(int mode)
    {
        if (enteredColliders.Length != selectedColliderId + mode && selectedColliderId + mode >= 0)
            selectedColliderId += mode;
    }

    private void FixedUpdate()
    {
        selectedEI = null;
        totalColliderName = "";
        totalColliderMode = "";
        spawnName = "";

        enteredColliders = Physics2D.RaycastAll(rayStart.position, Vector2.up, 6.5f, layerMaskInteract);
        interLabelText.text = "";
        if (enteredColliders.Length > 0 && !lockInter)
        {
            int idx = 0;
            foreach (RaycastHit2D hit in enteredColliders)
            {
                switch (hit.collider.tag)
                {
                    case "dialog":
                        scripts.dialogsManager.ActivateDialog(hit.collider.name);
                        break;
                    case "location":
                        totalColliderName = hit.collider.name;
                        totalColliderMode = hit.collider.tag;

                        selectedEI = hit.collider.gameObject.GetComponent<ExtraInteraction>().interactions[0];
                        spawnName = selectedEI.moveToSpawn;

                        if (!scripts.locations.GetLocation(totalColliderName).locked)
                        {
                            if (scripts.locations.GetLocation(totalColliderName).autoEnter && scripts.locations.totalLocation.autoEnter)
                                scripts.locations.ActivateLocation(totalColliderName, spawnName);
                            else
                                UpdateIntersMenu(idx, selectedEI.interLabel);
                        }
                        break;
                    case "cutscene":
                    case "interact":
                    case "item":
                        if (hit.collider.gameObject.GetComponent<ExtraInteraction>() != null)
                        {
                            ExtraInteraction EI = hit.collider.gameObject.GetComponent<ExtraInteraction>();
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
                                hit.collider.gameObject.name = totalEI.interName; // Сделано плохо
                                UpdateIntersMenu(idx, selectedEI.interLabel);
                                break;
                            }
                            if (selectedEI == null)
                                return;
                        }
                        totalColliderName = hit.collider.gameObject.name;
                        totalColliderMode = hit.collider.gameObject.tag;

                        break;
                    case "cutsceneAuto":
                        if (hit.collider.gameObject.GetComponent<ExtraInteraction>() != null)
                        {
                            extraInter totalEI = hit.collider.gameObject.GetComponent<ExtraInteraction>().interactions[0];
                            if (totalEI.nameQuestRequired != scripts.quests.totalQuest.nameInGame)
                            {
                                if (totalEI.nameQuestRequired != "")
                                    break;
                            }

                            if (totalEI.stageInter != scripts.quests.totalQuest.totalStep && totalEI.nameQuestRequired != "")
                                break;
                        }
                        scripts.cutsceneManager.ActivateCutscene(hit.collider.gameObject.name);
                        break;
                }
                if (hit.collider.name != "Player" && hit.collider.name != "Player")
                    idx++;
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ChoiceInter(1);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChoiceInter(-1);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (enteredColliders.Length > 0)
            {
                totalColliderName = enteredColliders.ElementAt(selectedColliderId).collider.gameObject.name;
                totalColliderMode = GameObject.Find(totalColliderName).tag;
                if (totalColliderMode != "location")
                {
                    if (enteredColliders.ElementAt(selectedColliderId).collider.gameObject.GetComponent<ExtraInteraction>() != null)
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
                        // TODO: в интеракциях, где можно много раз - это мешает.
                        //enteredColliders.Remove(enteredColliders.ElementAt(selectedColliderId).Key);
                        break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!scripts.main.CheckAnyMenuOpen())
                scripts.player.playerMenu.gameObject.SetActive(true);
            else
                scripts.player.playerMenu.gameObject.SetActive(false);
            scripts.player.canMove = !scripts.player.playerMenu.activeSelf;
        }
    }
}
