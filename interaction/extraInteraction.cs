using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class extraInteraction : MonoBehaviour
{
    public extraInter[] interactions = new extraInter[0];
}

[System.Serializable]
public class extraInter
{
    public string interName;
    [HideInInspector]
    public string interLabel
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return ruLabelName;
            else
                return enLabelName;
        }
    }
    public string ruLabelName, enLabelName;

    [Tooltip("ЕСЛИ ДЛЯ ПЕРЕХОДА В ЛОКАЦИЮ - имя спавна")] public string moveToSpawn;
    // Распространяется на tag interact и item
    [Header("QuestSettings")]
    [Tooltip("Требуемый квест для применения")] public string nameQuestRequired;
    [Tooltip("Стадия квеста для применения")] public int stageInter;
    [Tooltip("Переключение на следующий этап")] public bool NextStep;
    public bool activateCutscene;
    [Tooltip("Уничтожение после использования(item удаляется в любом случае)")] public bool destroyAfterInter;

    [Header("OtherSettings")]
    [Tooltip("Использование предмета")] public string itemNameUse;
    [Tooltip("Выдача предмета")] public string itemNameAdd;
    [Tooltip("Затемнение")] public bool darkAfterUse;
    [Tooltip("Смена внешки")] public bool swapPlayerVisual;
    [Tooltip("Номер внешки")] public int playerVisual;
}