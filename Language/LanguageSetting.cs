using UnityEngine;
using GameOptions;

[System.Serializable]
public class LanguageSetting
{
    [TextArea(0, 10)] [SerializeField] private string ru, en;

    public string Text
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

    public LanguageSetting(string anyText)
    {
        ru = anyText;
        en = anyText;
    }

    public LanguageSetting(string russianText, string englishText)
    {
        ru = russianText;
        en = englishText;
    }
}