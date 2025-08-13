using System.Threading.Tasks;
using UnityEngine;

public class DayProcess
{
    public static DayProcess Instance { get; private set; }
    public int Day = 1;
    public int Hour, Minute;
    public bool StopTime;
    public System.Action EveryHourAction;


    public Task Initialize()
    {
        Instance = this;
        MoveTime();
        return Task.CompletedTask;
    }

    private async Task ChangeTime()
    {
        if (StopTime) return;

        Minute++;
        float newHunger =
        PlayerStats.Hunger += 0.002f + (PlayerStats.Addiction * 0.002f);
        PlayerStats.Addiction += 0.00001f;

        if (Minute == 30)
        {
            Hour++;
            Minute = 0;
            EveryHourAction?.Invoke();
            if (Hour == 24)
            {
                Hour = 0;
                Day += 1;
                NpcManager.Instance.GenerateScheduleForAll();
            }
        }

        PlayerMenu.Instance.totalTimeText.text = StopTime ? "" : Hour.ToString("D2") + ":" + Minute.ToString("D2");
        PlayerMenu.Instance.totalDayText.text = StopTime ? "" : Day + new LanguageSetting(" день", " day").Text;
    }

    private async void MoveTime()
    {
        while (true)
        {
            float delay = 1.5f - PlayerStats.Addiction;
            delay = Mathf.Max(0, delay);

            await Task.Delay(Mathf.RoundToInt(delay * 1000 / Time.timeScale));
            await ChangeTime();
        }
    }
}