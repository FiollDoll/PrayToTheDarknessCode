using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Language : MonoBehaviour
{
    private string text => PlayerPrefs.GetString("language") == "ru" ? textRu : textEn;

    [TextArea][SerializeField] private string textRu, textEn;

    private void Start() => UpdateText();

    public void UpdateText()
    {
        if (gameObject.GetComponent<TextMeshProUGUI>())
            gameObject.GetComponent<TextMeshProUGUI>().text = text;
        if (gameObject.GetComponent<Text>())
            gameObject.GetComponent<Text>().text = text;
    }
}