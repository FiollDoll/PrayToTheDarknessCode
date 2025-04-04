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
    public static QuestsSystem Instance { get; private set; }

    [Header("Player quests")] public Quest totalQuest;
    public List<Quest> activeQuests = new List<Quest>();

    [Header("Game quests")] public Quest[] gameQuests = new Quest[0];
    private Dictionary<string, Quest> _gameQuestDict = new Dictionary<string, Quest>();

    [Header("UI objects")] [SerializeField]
    private GameObject questMenu;

    [SerializeField] private TextMeshProUGUI textQuest, textNameQuest;

    private float _timer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (Quest quest in gameQuests)
            _gameQuestDict.Add(quest.nameInGame, quest);

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
        //Notebook.Instance.ChoicePage(1);
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

        UpdateQuestUI();

        if (totalQuest == null) // Если новый квест не назначен
            return;

        if (totalQuest.steps[totalQuest.totalStep].delayNextStep != 0)
            StartCoroutine(StartStepDelay(totalQuest.steps[totalQuest.totalStep].delayNextStep));
        if (totalQuest.steps[totalQuest.totalStep].startDialog != "")
            DialogsManager.Instance.ActivateDialog(totalQuest.steps[totalQuest.totalStep].startDialog);
    }

    private void UpdateQuestUI()
    {
        questMenu.SetActive(totalQuest != null);
        if (!questMenu.activeSelf) return;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            _timer = 0;
            UpdateQuestUI();
        }

        _timer += 0.005f;
        if (_timer >= 1)
            questMenu.SetActive(false);
    }

    private IEnumerator StartStepDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextStep();
    }
}