using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactions : MonoBehaviour
{
    [SerializeField] private GameObject iconInteraction;
    [SerializeField] private allScripts scripts;
    private string totalColliderName, totalColliderMode, spawnName;

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
                if (scripts.locations.GetLocation(totalColliderName).autoEnter && scripts.locations.totalLocation.autoEnter)
                    scripts.locations.ActivateLocation(totalColliderName, spawnName);
                else
                    iconInteraction.gameObject.SetActive(true);
                break;
            case "interact":
            case "item":
                if (other.gameObject.GetComponent<extraInteraction>())
                {
                    if (other.gameObject.GetComponent<extraInteraction>().stageInter == scripts.quests.totalStep)
                    {
                        iconInteraction.gameObject.SetActive(true);
                        totalColliderName = other.gameObject.name;
                        totalColliderMode = other.gameObject.tag;
                    }
                }
                else
                {
                    iconInteraction.gameObject.SetActive(true);
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
                iconInteraction.gameObject.SetActive(false);
                break;
            case "location":
                iconInteraction.gameObject.SetActive(false);
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
                        Destroy(GameObject.Find(totalColliderName));
                        iconInteraction.gameObject.SetActive(false);
                        scripts.inventory.AddItem(totalColliderName);
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
