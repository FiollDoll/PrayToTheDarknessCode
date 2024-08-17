using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class extraInteraction : MonoBehaviour
{
    [Header("QuestSettings")]
    [Tooltip("Стадия квеста для применения")]public int stageInter;
    [Tooltip("Переключение на следующий этап")]public bool NextStep;

    [Header("OtherSettings")]
    [Tooltip("Смена спрайта")]public GameObject spriteObjChange;
    [Tooltip("Объект, у которого меняется спрайт")]public Sprite spriteChange;
    [Tooltip("Смена состояния видимости")]public bool setStateForObj;
    [Tooltip("Объект, у которого меняется состояние видимости")]public GameObject objWithState;
    [Tooltip("Смена внешки")]public bool swapPlayerVisual;
    [Tooltip("Номер внешки")]public int playerVisual;
    [Tooltip("Уничтожение после использования(не распространяется на предметы)")]public bool destroyAfterInter;

}
