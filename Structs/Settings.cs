using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CrimsonQuest.Structs;

public readonly struct Settings
{
    private readonly ConfigFile CONFIG;
    private readonly ConfigEntry<bool> ENABLE_MOD;

    public readonly ConfigEntry<int> MAX_DAILY;
    public readonly ConfigEntry<int> MAX_WEEKLY;

    public readonly ConfigEntry<int> DAY_OF_RESET;
    public readonly ConfigEntry<int> TIME_OF_RESET;

    public static readonly string CONFIG_PATH = Path.Combine(BepInEx.Paths.ConfigPath, "CrimsonQuest");

    public Settings(ConfigFile config)
    {
        CONFIG = config;
        ENABLE_MOD = CONFIG.Bind("Config", "EnableMod", true, "Enable or disable quest system");

        MAX_DAILY = CONFIG.Bind("Config", "MaxDailies", 5, "The max amount of daily quests a player can complete in one day");
        MAX_WEEKLY = CONFIG.Bind("Config", "MaxWeeklies", 3, "The max amount of weekly quests a player can complete in a week");

        DAY_OF_RESET = CONFIG.Bind("Config", "ResetDay", 1, "The day weeklies reset, 0 - Sunday, 1 - Monday, and so on.");
        TIME_OF_RESET = CONFIG.Bind("Config", "ResetTime", 0, "The hour on which Daily & Weekly resets in 24-hour time (0-23)");
    }

    public readonly void InitConfig()
    {
        WriteConfig();

        Plugin.LogInstance.LogInfo($"Mod enabled: {ENABLE_MOD.Value}");
    }

    public readonly void WriteConfig()
    {
        if (!Directory.Exists(CONFIG_PATH)) Directory.CreateDirectory(CONFIG_PATH);
    }

    public readonly bool GetActiveSystem(Systems type)
    {
        return type switch
        {
            Systems.ENABLE => ENABLE_MOD.Value,
            _ => false,
        };
    }
}
