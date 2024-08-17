using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class extraInteraction : MonoBehaviour
{
    // Распространяется на tag interact и item
    [Header("QuestSettings")]
    [Tooltip("Стадия квеста для применения")]public int stageInter;
    [Tooltip("Переключение на следующий этап")]public bool NextStep;

    [Header("ObjectsSettings")]
    [Tooltip("Смена спрайта")]public GameObject spriteObjChange;
    [Tooltip("Объект, у которого меняется спрайт")]public Sprite spriteChange;
    [Tooltip("Смена состояния видимости")]public bool setStateForObj;
    [Tooltip("Объект, у которого меняется состояние видимости")]public GameObject objWithState;
    [Tooltip("Уничтожение после использования(item удаляется в любом случае)")]public bool destroyAfterInter;

    [Header("OtherSettings")]

    [Tooltip("Смена внешки")]public bool swapPlayerVisual;
    [Tooltip("Номер внешки")]public int playerVisual;
}
