using UnityEngine;
using GameOptions;

[System.Serializable]
public class LanguageSetting
{
    [SerializeField] private string ru;
    [SerializeField] private string en;

    public string text
    {
        get
        {
            return Options.Language switch
            {
                Options.Languages.Ru => ru,
                Options.Languages.En => en,
                _ => "TEXT ERROR"
            };
        }
    }

    public LanguageSetting(string russianText, string englishText)
    {
        ru = russianText;
        en = englishText;
    }
}