using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class devTool : MonoBehaviour
{
    [SerializeField] private GameObject menuTools;
    [SerializeField] private TMP_InputField InputFieldStage, InputFieldNewQuest, InputFieldToLocation, InputFieldSpawn;
    [SerializeField] private allScripts scripts;

    public void ActivateMenuTools() => menuTools.gameObject.SetActive(!menuTools.activeSelf);


    public void ActivateQuest() => scripts.quests.ActivateQuest(InputFieldNewQuest.text);

    public void ActivateLocation() => scripts.locations.ActivateLocation(InputFieldToLocation.text, InputFieldSpawn.text);


    public void ActivateStepQuest()
    {
        foreach (quest quest in scripts.quests.gameQuests)
        {
            if (quest.nameInGame == scripts.quests.totalQuest.nameInGame) // Ссылки смешно работают
            {
                quest.totalStep = int.Parse(InputFieldStage.text);
                scripts.quests.UpdateQuestUI();
                break;
            }
        }
    }

    public void AddItem() => scripts.inventory.AddItem("");


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            ActivateMenuTools();
    }
}
