using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DevTool : MonoBehaviour
{
    public GameObject menuTools;
    [SerializeField] private TextMeshProUGUI textInfo;
    [SerializeField] private TMP_InputField InputFieldStage, InputFieldNewQuest, InputFieldToLocation, InputFieldSpawn;
    [SerializeField] private AllScripts scripts;

    public void ActivateMenuTools() => menuTools.gameObject.SetActive(!menuTools.activeSelf);

    public void ActivateQuest()
    {
        scripts.questsSystem.ActivateQuest(InputFieldNewQuest.text);
        InputFieldNewQuest.text = "";
    }
    public void ActivateLocation()
    {
        string gate = InputFieldSpawn.text;
        if (gate == "")
            gate = scripts.locations.GetLocation(InputFieldToLocation.text).spawns[0].name;
        scripts.locations.ActivateLocation(InputFieldToLocation.text, gate);
        InputFieldToLocation.text = "";
        InputFieldToLocation.text = "";
    }

    public void ActivateStepQuest()
    {
        foreach (Quest quest in scripts.questsSystem.gameQuests)
        {
            if (quest.nameInGame == scripts.questsSystem.totalQuest.nameInGame) // Ссылки смешно работают
            {
                quest.totalStep = int.Parse(InputFieldStage.text);
                scripts.questsSystem.UpdateQuestUI();
                break;
            }
        }
        InputFieldStage.text = "";
    }

    public void AddItem() => scripts.inventory.AddItem("");

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            ActivateMenuTools();
        textInfo.text = "dialogStep: " + scripts.dialogsManager.totalStep + "\ntotalLocation: " + scripts.locations.totalLocation.name + "\nplayerCanMove: " + scripts.player.canMove + "\ntotalQuest: " + scripts.questsSystem.totalQuest.name + "\n";
    }
}
