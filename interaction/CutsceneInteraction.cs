using UnityEngine;

public class CutsceneInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public string interName { get; set; }

    public string interLabel => PlayerPrefs.GetString("language") == "ru" ? ruLabelName : enLabelName;
    public string ruLabelName { get; set; }
    public string enLabelName { get; set; }
    public bool autoUse { get; set; }

    [Header("Requires")] public string nameQuestRequired { get; set; }
    public int stageInter { get; set; }

    [Header("Preferences")] public bool nextQuestStep { get; set; }

    private AllScripts _scripts;

    private void Awake()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    public void DoInteraction()
    {
        _scripts.cutsceneManager.ActivateCutscene(interName);
    }
}