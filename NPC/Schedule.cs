using UnityEngine;

[System.Serializable]
public class ScheduleCase
{
    [Header("Action")]
    public Enums.ScheduleAction action;
    public string actionTargetName, actionTargetLocation;

    [Header("Time")]
    public int dayStart;
    public int hourStart = -1;
    public int durationInMinutes;

    [Header("Change")]
    public bool stayOnPoint;
    public string changeStyle;
    public string startAnimationInPoint;
    public FastChangesController fastChangesController;
}