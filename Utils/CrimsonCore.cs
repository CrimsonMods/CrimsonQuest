using BepInEx.Logging;
using Bloodstone.API;
using CrimsonQuest.DB;
using CrimsonQuest.Hooks;
using System;
using System.Runtime.CompilerServices;
using Unity.Entities;

namespace CrimsonQuest.Utils;

internal static class CrimsonCore
{
    public static World Server { get; } = VWorld.Server;
    public static EntityManager EntityManager { get; } = Server.EntityManager;
    public static ManualLogSource Log = Plugin.LogInstance;
    public static QuestService Quest { get; internal set; }

    public static PlayerDatabase PlayerData { get; internal set; }
    public static Database QuestData { get; internal set; }

    // Check paramater
    private static bool _hasInitialized = false;

    public static void LogException(Exception e, [CallerMemberName] string caller = null)
    {
        Log.LogError($"Failure in {caller}\nMessage: {e.Message} Inner:{e.InnerException?.Message}\n\nStack: {e.StackTrace}\nInner Stack: {e.InnerException?.StackTrace}");
    }

    internal static void InitializeAfterLoaded()
    {
        if (_hasInitialized) return;

        QuestData = new Database();
        PlayerData = new PlayerDatabase();

        Quest = new();

        ResetScheduler.StartTimer();
        _hasInitialized = true;
        Log.LogInfo($"{nameof(InitializeAfterLoaded)} completed");
    }
}
