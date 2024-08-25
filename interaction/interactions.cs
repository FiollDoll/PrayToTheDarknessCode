using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class interactions : MonoBehaviour
{
    [SerializeField] private GameObject iconInteractionObj, floorChangeMenu;
    [SerializeField] private Sprite iconDefault, iconLocation;
    [SerializeField] private Image noViewPanel;
    [SerializeField] private allScripts scripts;
    private string totalColliderName, totalColliderMode, spawnName;
    extraInter selectedEI = null;

    private void IconInteractionActivate(bool status, int mode = 0)
    {
        iconInteractionObj.gameObject.SetActive(status);
        if (mode == 0)
            iconInteractionObj.GetComponent<Image>().sprite = iconDefault;
        else
            iconInteractionObj.GetComponent<Image>().sprite = iconLocation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "dialog":
                scripts.dialogsManager.ActivateDialog(other.gameObject.name);
                break;
            case "location":
                char[] nameLocationChar = other.gameObject.name.ToCharArray();
                totalColliderName = "";
                totalColliderMode = "location";
                spawnName = "";
                for (int i = 0; i < nameLocationChar.Length; i++)
                {
                    if (i != (nameLocationChar.Length - 1))
                        totalColliderName += nameLocationChar[i];
                    else
                        spawnName = nameLocationChar[i].ToString();
                }
                if (!scripts.locations.GetLocation(totalColliderName).locked)
                {
                    if (scripts.locations.GetLocation(totalColliderName).autoEnter && scripts.locations.totalLocation.autoEnter)
                        scripts.locations.ActivateLocation(totalColliderName, spawnName);
                    else
                        IconInteractionActivate(true, 1);
                }
                break;
            case "cutscene":
            case "interact":
            case "item":
                if (GameObject.Find(other.gameObject.name).GetComponent<extraInteraction>() != null)
                {
                    extraInteraction EI = GameObject.Find(other.gameObject.name).GetComponent<extraInteraction>();
                    for (int i = 0; i < EI.interactions.Length; i++)
                    {
                        extraInter totalEI = EI.interactions[i];
                        if (totalEI.nameQuestRequired != scripts.quests.totalQuest.nameEn)
                        {
                            if (totalEI.nameQuestRequired != "")
                                continue;
                        }

                        if (totalEI.stageInter != scripts.quests.totalQuest.totalStep && totalEI.nameQuestRequired != "")
                            continue;
                        selectedEI = EI.interactions[i];
                        GameObject.Find(other.gameObject.name).gameObject.name = totalEI.interName; // Сделано плохо
                        break;
                    }
                    if (selectedEI == null)
                        return;
                }
                IconInteractionActivate(true);
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
                IconInteractionActivate(false);
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

                switch (totalColliderMode)
                {
                    case "item":
                        IconInteractionActivate(false);
                        scripts.inventory.AddItem(totalColliderName);
                        Destroy(GameObject.Find(totalColliderName));
                        break;
                    case "location":
                        if (!scripts.locations.GetLocation(totalColliderName).locked)
                            scripts.locations.ActivateLocation(totalColliderName, spawnName);
                        break;
                    case "cutscene":
                        scripts.cutsceneManager.ActivateCutscene(totalColliderName);
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
                if (selectedEI != null)
                {
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
                    if (selectedEI.swapPlayerVisual)
                        scripts.player.ChangeVisual(selectedEI.playerVisual);
                    if (selectedEI.destroyAfterInter)
                        Destroy(GameObject.Find(totalColliderName));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            scripts.notebook.ManageNotePanel();
    }
}
