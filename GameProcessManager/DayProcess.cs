using System.Threading.Tasks;
using UnityEngine;

public class DayProcess
{
    public static DayProcess Instance { get; private set; }
    public int Day;
    public int Hour, Minute;
    public bool StopTime;

    public async Task Initialize()
    {
        Instance = this;
        MoveTime();
    }

    private async void MoveTime()
    {
        await Task.Delay(4000);
        if (StopTime) return;
        Minute++;
        if (Minute == 30)
        {
            Hour++;
            Minute = 0;
        }
    }
}