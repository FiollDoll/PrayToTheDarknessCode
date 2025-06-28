using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class NotifyManager : MonoBehaviour
{
    public static NotifyManager Instance { get; private set; }
    [SerializeField] private GameObject notifyContainer;
    [SerializeField] private GameObject notifyPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void StartNewItemNotify(string itemName) =>
        ActivateNotify(new LanguageSetting($"Новый предмет: {itemName}", $"New item: {itemName}"));

    public void StartNewNoteNotify(string noteName) =>
        ActivateNotify(new LanguageSetting($"Новая записка: {noteName}", $"New note: {noteName}"));

    public void StartNewRelationshipNotify(string npc, float changeRelationship)
    {
        switch (changeRelationship)
        {
            case > 0 and < 1:
                ActivateNotify(new LanguageSetting($"{npc} <color=green>></color>",
                    $"{npc} <color=green>></color>"));
                break;
            case >= 1:
                ActivateNotify(new LanguageSetting($"{npc} <color=green>>></color>",
                    $"{npc} <color=green>>></color>"));
                break;
            case < 0 and > -1:
                ActivateNotify(new LanguageSetting($"{npc} <color=green><</color>",
                    $"{npc} <color=green><</color>"));
                break;
            case >= -1:
                ActivateNotify(new LanguageSetting($"{npc} <color=red><<</color>",
                    $"{npc} <color=red><<</color>"));
                break;
        }
    }

    private async void ActivateNotify(LanguageSetting notifyText)
    {
        TextMeshProUGUI newNotifyText =
            Instantiate(notifyPrefab, notifyContainer.transform).GetComponent<TextMeshProUGUI>();
        newNotifyText.text = notifyText.text;
        await Task.Delay(5000);
        Destroy(newNotifyText.gameObject);
    }
}