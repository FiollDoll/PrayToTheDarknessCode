using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuestsManager : MonoBehaviour
{
    public static QuestsManager Instance { get; private set; }
    private Quest _selectedQuest;
    private int _selectedStep;
    [SerializeField] private Quest[] allQuests = new Quest[0];
    private Dictionary<string, Quest> _allQuestsDict = new Dictionary<string, Quest>();

    private void Awake() => Instance = this;
    
    public Task Initialize()
    {
        //foreach (Quest quest in allQuests)
            //_allQuestsDict.Add(quest.questName, quest);
        return Task.CompletedTask;
    }

    public QuestStep GetTotalQuestStep() => _selectedQuest?.questSteps[_selectedStep];

    public Task ActivateQuest(string name)
    {
        if (string.IsNullOrEmpty(name)) return Task.CompletedTask;
        _selectedQuest = _allQuestsDict[name];
        return Task.CompletedTask;
    }

    public void NextStep()
    {
        GetTotalQuestStep().changesAfterEnd.ActivateChanges();
        _selectedStep++;
        if (_selectedStep == _selectedQuest.questSteps.Length)
        {
            _selectedQuest = null;
            _selectedStep = 0;
        }
    }
}