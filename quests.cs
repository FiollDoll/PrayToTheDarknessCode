using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class quests : MonoBehaviour
{
    public quest totalQuest;
    public int totalStep;
    public List<quest> recentQuests = new List<quest>();
    [SerializeField] private quest[] gameQuests = new quest[0];
    [SerializeField] private TextMeshProUGUI textQuest, textNameQuest;
    [SerializeField] private allScripts scripts;

    private void Start() => ActivateQuest("Good morning");

    public void ActivateQuest(string name)
    {
        foreach (quest quest in gameQuests)
        {
            if (quest.nameEn == name)
            {
                recentQuests.Add(quest);
                UpdateQuestUI();
                break;
            }
        }
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
        totalStep++;
        Sequence sequence = DOTween.Sequence();

        Tween fadeAnimation = textQuest.gameObject.GetComponent<RectTransform>().DOAnchorPosX(-600, 0.5f).SetEase(Ease.InQuart);
        fadeAnimation.OnComplete(() =>
        {
            UpdateQuestUI();
        });
        sequence.Append(fadeAnimation);
        sequence.Append(textQuest.gameObject.GetComponent<RectTransform>().DOAnchorPosX(-340, 0.5f).SetEase(Ease.OutQuart));
        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
        if (totalQuest.steps[totalStep].delayNextStep != 0)
            StartCoroutine(StartStepDelay(totalQuest.steps[totalStep].delayNextStep));
        if (totalQuest.steps[totalStep].startDialog != "")
            scripts.dialogsManager.ActivateDialog(totalQuest.steps[totalStep].startDialog);
    }

    private void UpdateQuestUI()
    {
        totalQuest = recentQuests[recentQuests.Count - 1];
        textNameQuest.text = totalQuest.name;
        textQuest.text = totalQuest.steps[totalStep].name;
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
    public float delayNextStep;
    public string startDialog;
}
