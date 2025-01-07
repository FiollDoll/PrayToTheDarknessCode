using UnityEngine;

public class DialogInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public string ruLabelName;
    public string enLabelName;
    public string interLabel => PlayerPrefs.GetString("language") == "ru" ? ruLabelName : enLabelName;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;
    public string NameQuestRequired;
    public string nameQuestRequired => NameQuestRequired;
    public int StageInter;
    public int stageInter => StageInter;

    [Header("Preferences")] public bool nextQuestStep { get; set; }

    private AllScripts _scripts;

    private void Awake()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    public void DoInteraction()
    {
        _scripts.dialogsManager.ActivateDialog(this.gameObject.name);
    }
}