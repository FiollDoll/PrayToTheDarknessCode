using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class invSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int id;
    public inventory inv;
    [SerializeField] private GameObject textItem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if ((inv.playerItems.Count - 1) >= id)
        {
            textItem.gameObject.SetActive(true);
            textItem.GetComponent<TextMeshProUGUI>().text = inv.playerItems[id].name;            
        }
    }

    public void OnPointerExit(PointerEventData eventData) => textItem.gameObject.SetActive(false);

}
