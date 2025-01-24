using System.Collections;
using UnityEngine;
using TMPro;

public class NotifyManager : MonoBehaviour
{
    [SerializeField] private GameObject notifyContainer;
    [SerializeField] private GameObject notifyPrefab;

    // TODO: Гибкость языка + расширение
    public void StartNewItemNotify(string itemName) => StartCoroutine(ActivateNotify("Инвентарь: +" + itemName));
    
    public void StartNewNoteNotify(string noteName) => StartCoroutine(ActivateNotify("Записки: +" + noteName));

    private IEnumerator ActivateNotify(string notifyText)
    {
        TextMeshProUGUI newNotifyText = Instantiate(notifyPrefab, notifyContainer.transform).GetComponent<TextMeshProUGUI>();
        newNotifyText.text = notifyText;
        yield return new WaitForSeconds(5f);
        Destroy(newNotifyText.gameObject);
    }
}
