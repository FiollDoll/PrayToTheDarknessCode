using UnityEngine;
using TMPro;

public class DevTool : MonoBehaviour
{
    public GameObject menuTools;
    [SerializeField] private TextMeshProUGUI textInfo;
    [SerializeField] private TMP_InputField inputFieldStage, inputFieldNewQuest, inputFieldToLocation, inputFieldSpawn;
    private AllScripts _scripts;

    public void Initialize() => _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();

    public void ActivateMenuTools() => menuTools.gameObject.SetActive(!menuTools.activeSelf);

    public void ActivateQuest()
    {
        _scripts.questsSystem.ActivateQuest(inputFieldNewQuest.text);
        inputFieldNewQuest.text = "";
    }

    public void ActivateLocation()
    {
        string gate = inputFieldSpawn.text;
        if (gate == "")
            gate = _scripts.manageLocation.GetLocation(inputFieldToLocation.text).spawns[0].name;
        _scripts.manageLocation.ActivateLocation(inputFieldToLocation.text, gate);
        inputFieldToLocation.text = "";
        inputFieldToLocation.text = "";
    }

    public void ActivateStepQuest()
    {
        foreach (Quest quest in _scripts.questsSystem.gameQuests)
        {
            if (quest.nameInGame == _scripts.questsSystem.totalQuest.nameInGame) // Ссылки смешно работают
            {
                quest.totalStep = int.Parse(inputFieldStage.text);
                _scripts.questsSystem.UpdateQuestUI();
                break;
            }
        }

        inputFieldStage.text = "";
    }

    public void AddItem() => _scripts.inventoryManager.AddItem("");

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            ActivateMenuTools();
        textInfo.text = "dialogStep: " + _scripts.dialogsManager.totalStep + "\ntotalLocation: " +
                        _scripts.manageLocation.totalLocation.name + "\nplayerCanMove: " + _scripts.player.canMove +
                        "\ntotalQuest: " + _scripts.questsSystem.totalQuest.name + "\n";
    }
}