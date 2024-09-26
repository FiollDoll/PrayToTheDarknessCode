using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class interactions : MonoBehaviour
{
    [SerializeField] private GameObject floorChangeMenu;
    [SerializeField] private Sprite iconDefault, iconLocation;
    [SerializeField] private TextMeshProUGUI interLabelText;
    [SerializeField] private Image noViewPanel;
    [SerializeField] private allScripts scripts;
    private string totalColliderName, totalColliderMode, spawnName;
    public extraInter selectedEI = null;

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
                        interLabelText.text = selectedEI.interLabel;
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
                        interLabelText.text = selectedEI.interLabel;
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
                interLabelText.text = "";
                break;
        }
        if (floorChangeMenu.gameObject.activeSelf)
            floorChangeMenu.gameObject.SetActive(false);
        selectedEI = null;
        totalColliderName = "";
        totalColliderMode = "";
        spawnName = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (totalColliderName != "")
            {
                if (totalColliderMode != "location")
                {
                    if (GameObject.Find(totalColliderName).GetComponent<extraInteraction>() != null)
                    {
                        if (selectedEI == null)
                            return;
                    }
                }
                if (selectedEI != null)
                {
                    scripts.inventory.AddItem(selectedEI.itemNameAdd);
                    if (selectedEI.darkAfterUse)
                    {
                        Sequence sequence = DOTween.Sequence();
                        Tween fadeAnimation = noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart);
                        sequence.Append(fadeAnimation);
                        sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
                        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
                    }
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

                        interLabelText.text = "";
                        Destroy(GameObject.Find(totalColliderName));
                        break;
                    case "location":
                        if (!scripts.locations.GetLocation(totalColliderName).locked)
                            scripts.locations.ActivateLocation(totalColliderName, selectedEI.moveToSpawn);
                        break;
                    default:
                        if (totalColliderName == "floorChange")
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

        if (Input.GetKeyDown(KeyCode.Tab))
            scripts.notebook.ManageNotePanel();
    }
}
