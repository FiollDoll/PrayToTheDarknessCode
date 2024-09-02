using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class quests : MonoBehaviour
{
    public quest totalQuest;
    public List<quest> activeQuests = new List<quest>();
    [SerializeField] private quest[] gameQuests = new quest[0];
    [SerializeField] private TextMeshProUGUI textQuest, textNameQuest;
    [SerializeField] private allScripts scripts;

    private void Start() => ActivateQuest("Good morning");

    public void ActivateQuest(string name)
    {
        quest newQuest = FindQuest(name);
        if (newQuest != null)
        {
            if (totalQuest != null)
                totalQuest = newQuest;
            activeQuests.Add(newQuest);
            UpdateQuestUI();
        }
    }

    public void ChoiceActiveQuest(string name)
    {
        totalQuest = FindQuest(name);
        UpdateQuestUI();
        scripts.notebook.ChoicePage(0);
    }

    public quest FindQuest(string name)
    {
        foreach (quest quest in gameQuests)
        {
            if (quest.nameEn == name)
                return quest;
        }
        return null;
    }

    public void NextStep()
    {
        totalQuest.totalStep++;
        if (totalQuest.totalStep == totalQuest.steps.Length) // Окончание
        {
            activeQuests.Remove(totalQuest);
            if (activeQuests.Count != 0)
                totalQuest = activeQuests[activeQuests.Count - 1];
            else
                totalQuest = null;
        }
        
        Sequence sequence = DOTween.Sequence();

        Tween fadeAnimation = textQuest.gameObject.GetComponent<RectTransform>().DOAnchorPosX(-600, 0.5f).SetEase(Ease.InQuart);
        fadeAnimation.OnComplete(() =>
        {
            UpdateQuestUI();
        });
        sequence.Append(fadeAnimation);
        sequence.Append(textQuest.gameObject.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutQuart));
        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
        
        if (totalQuest == null)
            return;
            
        if (totalQuest.steps[totalQuest.totalStep].delayNextStep != 0)
            StartCoroutine(StartStepDelay(totalQuest.steps[totalQuest.totalStep].delayNextStep));
        if (totalQuest.steps[totalQuest.totalStep].startDialog != "")
            scripts.dialogsManager.ActivateDialog(totalQuest.steps[totalQuest.totalStep].startDialog);
    }

    private void UpdateQuestUI()
    {
        if (totalQuest != null)
        {
            textNameQuest.text = totalQuest.name;
            textQuest.text = totalQuest.steps[totalQuest.totalStep].name;
        }
        else
        {
            textNameQuest.text = "";
            textQuest.text = "";
        }
    }

    private IEnumerator StartStepDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextStep();
    }
}

[System.Serializable]
public class quest
{
    public int questId;
    public string nameRu, nameEn;
    [HideInInspector]
    public string name
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return nameRu;
            else
                return nameEn;
        }
    }

    public string descriptionRu, descriptionEn;
    [HideInInspector]
    public string description
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return descriptionRu;
            else
                return descriptionEn;
        }
    }
    public step[] steps = new step[0];
    public int totalStep;
}

[System.Serializable]
public class step
{
    public string nameRu, nameEn;
    [HideInInspector]
    public string name
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return nameRu;
            else
                return nameEn;
        }
    }
    public string descriptionRu, descriptionEn;
    [HideInInspector]
    public string description
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return descriptionRu;
            else
                return descriptionEn;
        }
    }
    public float delayNextStep;
    public string startDialog;
}
