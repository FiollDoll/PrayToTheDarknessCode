using System.Collections;
using UnityEngine;
using TMPro;

public class NotifyManager : MonoBehaviour
{
    [SerializeField] private GameObject notifyContainer;
    [SerializeField] private GameObject notifyPrefab;

    public void StartNewItemNotify(string itemName) =>
        StartCoroutine(ActivateNotify(new LanguageSetting($"Новый предмет: {itemName}", $"New item: {itemName}")));

    public void StartNewNoteNotify(string noteName) =>
        StartCoroutine(ActivateNotify(new LanguageSetting($"Новая записка: {noteName}", $"New note: {noteName}")));

    private IEnumerator ActivateNotify(LanguageSetting notifyText)
    {
        TextMeshProUGUI newNotifyText =
            Instantiate(notifyPrefab, notifyContainer.transform).GetComponent<TextMeshProUGUI>();
        newNotifyText.text = notifyText.text;
        yield return new WaitForSeconds(5f);
        Destroy(newNotifyText.gameObject);
    }
}