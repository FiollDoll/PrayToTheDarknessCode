using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuestsManager
{
    public static QuestsManager Instance { get; private set; }

    private Quest[] _allQuests = new Quest[0];
    private readonly Dictionary<string, Quest> _allQuestsDict = new Dictionary<string, Quest>();
    private Quest _selectedQuest;
    private int _selectedStep;

    public Task Initialize()
    {
        Instance = this;
        _allQuests = Resources.LoadAll<Quest>("Quests");
        foreach (Quest quest in _allQuests)
            _allQuestsDict.Add(quest.questName, quest);
        return Task.CompletedTask;
    }

    public QuestStep GetTotalQuestStep() => _selectedQuest?.questSteps[_selectedStep];

    public Task ActivateQuest(string name)
    {
        if (string.IsNullOrEmpty(name)) return Task.CompletedTask;
        _selectedQuest = _allQuestsDict[name];
        return Task.CompletedTask;
    }

    public async void NextStep()
    {
        await GetTotalQuestStep().changesAfterEnd.ActivateChanges();
        _selectedStep++;

        if (_selectedStep == _selectedQuest.questSteps.Length)
        {
            _selectedQuest = null;
            _selectedStep = 0;
        }
    }
}