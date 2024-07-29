using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class quests : MonoBehaviour
{
    public quest totalQuest;
    public int totalStep;
    public List<quest> recentQuests = new List<quest>();
    [SerializeField] private quest[] gameQuests = new quest[0];
    [SerializeField] private TextMeshProUGUI textQuest, textNameQuest;
    [SerializeField] private allScripts scripts;

    private void Start() => ActivateQuest("Доброе утро");

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
        scripts.quests.totalStep++;
        scripts.quests.UpdateQuestUI();
        UpdateQuestUI();
    }

    private void UpdateQuestUI()
    {
        totalQuest = recentQuests[recentQuests.Count - 1];
        textNameQuest.text = totalQuest.name;
        textQuest.text = totalQuest.steps[totalStep].name;
    }
}

[System.Serializable]
public class quest
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
}
