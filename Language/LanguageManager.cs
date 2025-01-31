using UnityEngine;
using GameOptions;

public class LanguageManager : MonoBehaviour
{
    [SerializeField] private Options.Languages language;
    
    private void Start() =>  Options.SetLanguage(language);
    
    private void OnValidate() => Options.SetLanguage(language);

    public void ChangeLanguage(Options.Languages newLanguage)
    {
        language = newLanguage;
        Options.SetLanguage(language);
    }
}