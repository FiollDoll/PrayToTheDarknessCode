using UnityEngine;

[System.Serializable]
public class Language
{
    [HideInInspector] public LanguageManager languageManager; // TODO: проблема назначения

    public string text
    {
        get
        {
            return languageManager.language switch
            {
                LanguageManager.Languages.Ru => ru,
                LanguageManager.Languages.En => en,
                _ => "TEXT ERROR"
            };
        }
    }

    [SerializeField] [TextArea] private string ru, en;

    public Language(string russian, string english)
    {
        ru = russian;
        en = english;
    }
}