using System.Collections.Generic;
using UnityEngine;

public class LanguageManager: MonoBehaviour
{
    public enum Languages
    {
        Ru,
        En
    };

    public Languages language;

    private List<LanguageText> _allLanguageTexts;

    private void Start()
    {
        LanguageText[] languagesObj = FindObjectsOfType<LanguageText>();
        // TODO: почему-то цикл не срабатывает
        foreach (LanguageText lang in languagesObj)
        {
            lang.language.languageManager = this;
            lang.UpdateText();
            _allLanguageTexts.Add(lang);
        }
    }
    
    public void UpdateAllTexts()
    {
        foreach (LanguageText lang in _allLanguageTexts)
            lang.UpdateText();
    }
}