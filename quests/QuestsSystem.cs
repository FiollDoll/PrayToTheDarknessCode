using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using LastFramework;
using Random = UnityEngine.Random;

public class QuestsSystem : MonoBehaviour
{
    public static QuestsSystem singleton { get; private set; }
    public Quest totalQuest;
    public List<Quest> activeQuests = new List<Quest>();
    public Quest[] gameQuests = new Quest[0];
    private Dictionary<string, Quest> _gameQuestDict = new Dictionary<string, Quest>();
    [SerializeField] private TextMeshProUGUI textQuest, textNameQuest;
    private RectTransform _textQuestTransform;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        foreach (Quest quest in gameQuests)
            _gameQuestDict.Add(quest.nameInGame, quest);
        
        _textQuestTransform = textQuest.GetComponent<RectTransform>();
        ActivateQuest("Good morning");
    }

    /// <summary>
    /// Возрат квеста по имени
    /// </summary>
    /// <param name="questName"></param>
    /// <returns></returns>
    public Quest GetQuest(string questName)
    {
        return _gameQuestDict.GetValueOrDefault(questName);
    }
    
    /// <summary>
    /// Активирует новый квест
    /// </summary>
    /// <param name="questName">Имя квеста</param>
    /// <param name="extraActivate">Перекрыть текущий АКТИВНЫЙ квест</param>
    public void ActivateQuest(string questName, bool extraActivate = false)
    {
        Quest newQuest = GetQuest(questName);
        if (newQuest == null) return;
        
        if (totalQuest != null || extraActivate)
            totalQuest = newQuest;
        activeQuests.Add(newQuest);
        UpdateQuestUI();
    }

    /// <summary>
    /// Выбор другого активного квеста
    /// </summary>
    /// <param name="questName"></param>
    public void ChoiceActiveQuest(string questName)
    {
        totalQuest = GetQuest(questName);
        UpdateQuestUI();
        //Notebook.singleton.ChoicePage(1);
    }

    /// <summary>
    /// Следующий этап квеста
    /// </summary>
    public void NextStep()
    {
        if (totalQuest == null)
            return;
        totalQuest.totalStep++;
        if (totalQuest.totalStep == totalQuest.steps.Length) // Окончание
        {
            activeQuests.Remove(totalQuest);
            totalQuest = activeQuests.Count != 0 ? activeQuests[^1] : null;
        }
        
        Sequence sequence = DOTween.Sequence();
        Tween textMoveAnimation = _textQuestTransform.DOAnchorPosX(-600, 0.5f)
            .SetEase(Ease.InQuart);
        textMoveAnimation.OnComplete(UpdateQuestUI);
        sequence.Append(textMoveAnimation);
        sequence.Append(_textQuestTransform.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutQuart));
        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));

        if (totalQuest == null) // Если новый квест не назначен
            return;

        if (totalQuest.steps[totalQuest.totalStep].delayNextStep != 0)
            StartCoroutine(StartStepDelay(totalQuest.steps[totalQuest.totalStep].delayNextStep));
        if (totalQuest.steps[totalQuest.totalStep].startDialog != "")
            DialogsManager.singleton.ActivateDialog(totalQuest.steps[totalQuest.totalStep].startDialog);
    }

    private void UpdateQuestUI()
    {
        if (totalQuest != null)
        {
            if (totalQuest.cursedText)
            {
                TextManager.SetCursedText(textNameQuest, Random.Range(5, 15));
                TextManager.SetCursedText(textQuest, Random.Range(5, 15));
            }
            else
            {
                TextManager.EndCursedText(textNameQuest);
                TextManager.EndCursedText(textQuest);
                textNameQuest.text = totalQuest.name.text;
                textQuest.text = totalQuest.steps[totalQuest.totalStep].name.text;
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