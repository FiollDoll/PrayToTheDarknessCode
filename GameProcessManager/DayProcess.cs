using System.Collections;
using UnityEngine;

public class DayProcess
{
    public static DayProcess Instance { get; private set; }
    public int Day;
    public int Hour, Minute;
    public bool StopTime;

    private CoroutineContainer _coroutineContainer;

    public void Initialize()
    {
        Instance = this;
        _coroutineContainer = CoroutineContainer.Instance;
        _coroutineContainer.StartCoroutine(MoveTime());
    }

    private IEnumerator MoveTime()
    {
        yield return new WaitForSeconds(4);
        if (StopTime) yield break;
        Minute++;
        if (Minute == 30)
        {
            Hour++;
            Minute = 0;
        }
    }
}