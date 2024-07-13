using BepInEx;
using Bloody.Core.Models.v1;
using CrimsonQuest.DB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CrimsonQuest.DB;

internal class PlayerDatabase
{
    public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "CrimsonQuest");
    public static string QuestTrackerFile = Path.Combine(ConfigPath, "quest_tracker.json");

    public List<QuestProgressModel> PlayerProgress { get; set; }

    public PlayerDatabase()
    {
        CreateDatabaseFiles();
        LoadDatabase();
    }

    public bool SaveDatabase()
    {
        try
        {
            var json = JsonSerializer.Serialize(PlayerProgress, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(QuestTrackerFile, json);
            Plugin.LogInstance.LogInfo($"Save Tracker Database: OK");
            return true;
        }
        catch (Exception e)
        {
            Plugin.LogInstance.LogError($"Error Save Tracker Database: {e.Message}");
            return false;
        }
    }

    public bool LoadDatabase()
    {
        try
        {
            string json = File.ReadAllText(QuestTrackerFile);
            PlayerProgress = JsonSerializer.Deserialize<List<QuestProgressModel>>(json);
            Plugin.LogInstance.LogInfo($"Load Tracker Database: OK");

            return true;
        }
        catch (Exception e)
        {
            Plugin.LogInstance.LogError($"Error Load Tracker Database: {e.Message}");
            return false;
        }
    }

    public bool CreateDatabaseFiles()
    {
        if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);
        if (!File.Exists(QuestTrackerFile)) File.WriteAllText(QuestTrackerFile, "[]");
        Plugin.LogInstance.LogInfo($"Create Tracker Database: OK");
        return true;
    }

    public bool GetProgressForUser(UserModel player, out QuestProgressModel progress)
    {
        Plugin.LogInstance.LogInfo($"Searching for {player.CharacterName}'s progress.");
        if (PlayerProgress.Exists(x => x.character == player.CharacterName))
        {
            progress = PlayerProgress.FirstOrDefault(x => x.character == player.CharacterName);
            return true;
        }
        else
        {
            progress = new QuestProgressModel(player.CharacterName);
            PlayerProgress.Add(progress);
            SaveDatabase();
            return false;
        }
    }

    public void UpdatePlayer(UserModel player, QuestProgressModel progress)
    {
        if (GetProgressForUser(player, out QuestProgressModel _))
        {
            progress.CompletedWeekly += progress.WeeklyQuests.Where(x => x.isCompleted).Count();
            progress.CompletedDaily += progress.DailyQuests.Where(x => x.isCompleted).Count();

            progress.WeeklyQuests = progress.WeeklyQuests.Where(x => x.isCompleted == false).ToList();
            progress.DailyQuests = progress.DailyQuests.Where(x => x.isCompleted == false).ToList();

            if (progress.StoryQuest.isCompleted)
            {
                progress.CompletedStories.Add(progress.StoryQuest.QuestInProgress.ID);
                progress.StoryQuest = null;
            }

            SaveDatabase();
        }
    }

    public void Reset(bool weekly = false)
    { 
        foreach (QuestProgressModel progress in PlayerProgress) 
        {
            progress.CompletedDaily = 0;

            if (weekly)
            {
                progress.CompletedWeekly = 0;
            }
        }
    }
}
