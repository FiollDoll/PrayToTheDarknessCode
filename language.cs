using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Language : MonoBehaviour
{
    private string text
    {
        get { return PlayerPrefs.GetString("language") == "ru" ? textRu : textEn; }
    }

    [SerializeField] private string textRu, textEn;

    private void Start() => UpdateText();

    public void UpdateText()
    {
        if (gameObject.GetComponent<TextMeshProUGUI>())
            gameObject.GetComponent<TextMeshProUGUI>().text = text;
        if (gameObject.GetComponent<Text>())
            gameObject.GetComponent<Text>().text = text;
    }
}