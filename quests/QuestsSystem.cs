using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class QuestsSystem : MonoBehaviour
{
    public Quest totalQuest;
    public List<Quest> activeQuests = new List<Quest>();
    public Quest[] gameQuests = new Quest[0];
    [SerializeField] private TextMeshProUGUI textQuest, textNameQuest;
    [SerializeField] private AllScripts scripts;

    // Отключено для разработки
    //private void Start() => ActivateQuest("FindFamily");
    private void Start() => ActivateQuest("Good morning");

    public void ActivateQuest(string questName, bool extraActivate = false)
    {
        Quest newQuest = FindQuest(questName);
        if (newQuest != null)
        {
            if (totalQuest != null || extraActivate)
                totalQuest = newQuest;
            activeQuests.Add(newQuest);
            UpdateQuestUI();
        }
    }

    public void ChoiceActiveQuest(string questName)
    {
        totalQuest = FindQuest(questName);
        UpdateQuestUI();
        scripts.notebook.ChoicePage(1);
    }

    public Quest FindQuest(string questName)
    {
        foreach (Quest quest in gameQuests)
        {
            if (quest.nameInGame == questName)
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

        Tween fadeAnimation = textQuest.gameObject.GetComponent<RectTransform>().DOAnchorPosX(-600, 0.5f)
            .SetEase(Ease.InQuart);
        fadeAnimation.OnComplete(UpdateQuestUI);
        sequence.Append(fadeAnimation);
        sequence.Append(
            textQuest.gameObject.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutQuart));
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