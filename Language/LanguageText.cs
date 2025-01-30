using UnityEngine;
using TMPro;

public class LanguageText : MonoBehaviour
{
    public Language language;
    private TextMeshProUGUI _textMeshProUGUI;
    
    private void Start() => _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    
    public void UpdateText() => _textMeshProUGUI.text = language.text;
    
}