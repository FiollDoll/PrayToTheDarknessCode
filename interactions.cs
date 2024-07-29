using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactions : MonoBehaviour
{
    [SerializeField] private GameObject iconInteraction;
    [SerializeField] private allScripts scripts;
    private string totalColliderName;
    private string totalColliderMode;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "dialog")
        {
            scripts.dialogsManager.ActivateDialog(other.gameObject.name);
        }
        else if (other.gameObject.tag == "interact" || other.gameObject.tag == "item")
        {
            iconInteraction.gameObject.SetActive(true);
            totalColliderName = other.gameObject.name;
            totalColliderMode = other.gameObject.tag;
        }
        else if (other.gameObject.tag == "location")
        {
            char[] nameLocationChar = other.gameObject.name.ToCharArray();
            string nameLocation = "";
            string spawnName = "";
            for (int i = 0; i < nameLocationChar.Length; i++)
            {
                if (i != (nameLocationChar.Length - 1))
                    nameLocation += nameLocationChar[i];
                else
                    spawnName = nameLocationChar[i].ToString();
            }
            scripts.locations.ActivateLocation(nameLocation, spawnName);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "interact")
            iconInteraction.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (totalColliderMode != "item")
            {
                if (!scripts.dialogsManager.dialogMenu.activeSelf)
                    scripts.dialogsManager.ActivateDialog(totalColliderName);
            }
            else
            {
                Destroy(GameObject.Find(totalColliderName));
                iconInteraction.gameObject.SetActive(false);
                scripts.inventory.AddItem(totalColliderName, true);// Переделать потом на questItem
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scripts.notebook.ManageNotePanel();
        }
    }
}
