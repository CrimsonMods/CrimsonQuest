using CrimsonQuest.Utils;
using System;

namespace CrimsonQuest.Hooks;

internal class ResetScheduler
{
    public static Action action;
    public static DateTime lastDateMinute = DateTime.Now;

    private static DayOfWeek DayOfWeekly;
    private static int TargetHour; 


    private static void CheckResets()
    {
        var date = DateTime.Now;

        if (date.Hour == TargetHour && date.Minute == 0)
        {
            if (date.DayOfWeek != DayOfWeekly)
            {
                CrimsonCore.PlayerData.Reset();
            }
            else
            {
                CrimsonCore.PlayerData.Reset(true);
            }
        }
    }

    public static void StartTimer()
    {
        switch (Plugin.Settings.DAY_OF_RESET.Value)
        {
            case 0:
                DayOfWeekly = DayOfWeek.Sunday; 
                break;
            case 1:
                DayOfWeekly = DayOfWeek.Monday;
                break;
            case 2:
                DayOfWeekly = DayOfWeek.Tuesday;
                break;
            case 3:
                DayOfWeekly= DayOfWeek.Wednesday;
                break;
            case 4:
                DayOfWeekly = DayOfWeek.Thursday;
                break;
            case 5:
                DayOfWeekly = DayOfWeek.Friday;
                break;
            case 6:
                DayOfWeekly = DayOfWeek.Saturday;
                break;
        }

        TargetHour = Plugin.Settings.TIME_OF_RESET.Value;

        Plugin.LogInstance.LogInfo($"Start Reset Timer for CrimsonQuest | Reset Hour: {TargetHour} | Weekly Reset Day: {DayOfWeekly.ToString()}");
        action = () =>
        {
            var date = DateTime.Now;
            if (lastDateMinute.ToString("HH:mm") != date.ToString("HH:mm"))
            {
                CheckResets();
            }
            ActionSchedulerPatch.RunActionOnceAfterFrames(action, 60);
        };
        ActionSchedulerPatch.RunActionOnceAfterFrames(action, 60);

    }
}
