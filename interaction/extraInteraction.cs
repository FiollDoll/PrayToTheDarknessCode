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
    public string interLabel;
    // Распространяется на tag interact и item
    [Header("QuestSettings")]
    [Tooltip("Требуемый квест для применения")] public string nameQuestRequired;
    [Tooltip("Стадия квеста для применения")] public int stageInter;
    [Tooltip("Переключение на следующий этап")] public bool NextStep;

    [Tooltip("Уничтожение после использования(item удаляется в любом случае)")] public bool destroyAfterInter;

    [Header("OtherSettings")]
    [Tooltip("Использование предмета")] public string itemNameUse;
    [Tooltip("Затемнение")] public bool darkAfterUse;
    [Tooltip("Смена внешки")] public bool swapPlayerVisual;
    [Tooltip("Номер внешки")] public int playerVisual;
}