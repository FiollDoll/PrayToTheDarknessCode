using System.Collections;
using UnityEngine;

public class DayProcess : MonoBehaviour
{
    [Header("Игровые переменные")] public int day;
    public int hour, minute;
    public bool stopTime;

    private void Start() => StartCoroutine(MoveTime());

    private IEnumerator MoveTime()
    {
        yield return new WaitForSeconds(4);
        if (stopTime) yield break;
        minute++;
        if (minute == 30)
        {
            hour++;
            minute = 0;
        }

        // Пока так
        //ChapterManager.Instance.LoadChapter(ChapterManager.Instance.GetChapterByName("prehistory"));
    }
}