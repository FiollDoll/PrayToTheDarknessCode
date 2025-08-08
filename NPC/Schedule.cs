
[System.Serializable]
public class ScheduleCase
{
    public Enums.ScheduleAction action;
    public int dayStart, hourStart = -1;
    public int durationInMinutes;
    public FastChangesController fastChangesController;
}