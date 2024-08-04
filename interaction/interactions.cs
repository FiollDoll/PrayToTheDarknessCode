using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class interactions : MonoBehaviour
{
    [SerializeField] private GameObject iconInteractionObj;
    [SerializeField] private Sprite iconDefault, iconLocation;
    [SerializeField] private allScripts scripts;
    private string totalColliderName, totalColliderMode, spawnName;

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
            case "interact":
            case "item":
                if (other.gameObject.GetComponent<extraInteraction>())
                {
                    if (other.gameObject.GetComponent<extraInteraction>().stageInter == scripts.quests.totalStep)
                    {
                        IconInteractionActivate(true);
                        totalColliderName = other.gameObject.name;
                        totalColliderMode = other.gameObject.tag;
                    }
                }
                else
                {
                    IconInteractionActivate(true);
                    totalColliderName = other.gameObject.name;
                    totalColliderMode = other.gameObject.tag;
                }
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "interact":
            case "item":
            case "location":
                IconInteractionActivate(false);
                break;
        }
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
                switch (totalColliderMode)
                {
                    case "item":
                        IconInteractionActivate(false);
                        scripts.inventory.AddItem(totalColliderName);
                        Destroy(GameObject.Find(totalColliderName));
                        break;
                    case "location":
                        scripts.locations.ActivateLocation(totalColliderName, spawnName);
                        break;
                    default:
                        if (!scripts.dialogsManager.dialogMenu.activeSelf)
                            scripts.dialogsManager.ActivateDialog(totalColliderName);
                        break;
                }

                if (GameObject.Find(totalColliderName).GetComponent<extraInteraction>())
                {
                    extraInteraction EI = GameObject.Find(totalColliderName).GetComponent<extraInteraction>();
                    if (EI.NextStep && EI.stageInter == scripts.quests.totalStep)
                        scripts.quests.NextStep();
                    if (EI.swapPlayerVisual)
                        scripts.player.ChangeVisual(EI.playerVisual);
                    if (EI.destroyAfterInter)
                        Destroy(GameObject.Find(totalColliderName));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            scripts.notebook.ManageNotePanel();
    }
}
