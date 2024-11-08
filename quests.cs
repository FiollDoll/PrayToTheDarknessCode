using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class quests : MonoBehaviour
{
    public quest totalQuest;
    public List<quest> activeQuests = new List<quest>();
    public quest[] gameQuests = new quest[0];
    [SerializeField] private TextMeshProUGUI textQuest, textNameQuest;
    [SerializeField] private allScripts scripts;

    private void Start() => ActivateQuest("FindFamily");

    public void ActivateQuest(string name, bool extraActivate = false)
    {
        quest newQuest = FindQuest(name);
        if (newQuest != null)
        {
            if (totalQuest != null || extraActivate)
                totalQuest = newQuest;
            activeQuests.Add(newQuest);
            UpdateQuestUI();
        }
    }

    public void ChoiceActiveQuest(string name)
    {
        totalQuest = FindQuest(name);
        UpdateQuestUI();
        scripts.notebook.ChoicePage(1);
    }

    public quest FindQuest(string name)
    {
        foreach (quest quest in gameQuests)
        {
            if (quest.nameInGame == name)
                return quest;
        }
        return null;
    }

    public void NextStep()
    {
        if (totalQuest == null)
            return;
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

    public void UpdateQuestUI()
    {
        if (totalQuest != null)
        {
            if (totalQuest.cursedText)
            {
                scripts.main.SetCursedText(textNameQuest, Random.Range(5, 15));
                scripts.main.SetCursedText(textQuest, Random.Range(5, 15));
            }
            else
            {
                scripts.main.EndCursedText(textNameQuest);
                scripts.main.EndCursedText(textQuest);
                textNameQuest.text = totalQuest.name;
                textQuest.text = totalQuest.steps[totalQuest.totalStep].name;
            }
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
    public string nameInGame;
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
    public bool cursedText;
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
