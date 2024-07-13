using BepInEx;
using CrimsonQuest.DB.Models;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CrimsonQuest.DB;

internal class Database
{
    public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "CrimsonQuest");
    public static string QuestGiverFile = Path.Combine(ConfigPath, "quest_givers.json");
    public static string QuestsFile = Path.Combine(ConfigPath, "quest_list.json");

    public List<QuestGiverModel> QuestGivers { get; set; }
    private List<QuestModel> Quests { get; set; }
    public List<QuestModel> DailyQuests { get; set; }
    public List<QuestModel> WeeklyQuests { get; set; }
    public List<QuestModel> TimedQuests { get; set; }
    public List<QuestModel> StoryQuests { get; set; }
    public List<QuestModel> WorldQuests { get; set; }

    public Database()
    {
        CreateDatabaseFiles();
        LoadDatabase();
    }

    public bool SaveDatabase()
    {
        try
        {
            var json = JsonSerializer.Serialize(QuestGivers, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(QuestGiverFile, json);
            Plugin.LogInstance.LogInfo($"Save Database: OK");
            return true;
        }
        catch (Exception e)
        {
            Plugin.LogInstance.LogError($"Error Save Database: {e.Message}");
            return false;
        }
    }

    public bool LoadDatabase()
    {
        try
        {
            string json = File.ReadAllText(QuestGiverFile);
            QuestGivers = JsonSerializer.Deserialize<List<QuestGiverModel>>(json);
            Plugin.LogInstance.LogInfo($"Load Quest Giver Database: OK");

            string q_json = File.ReadAllText(QuestsFile);
            Quests = JsonSerializer.Deserialize<List<QuestModel>>(q_json);
            Plugin.LogInstance.LogInfo($"Load Quests Database: OK");

            WeeklyQuests = Quests.Where(x => x.Type == QuestType.WEEKLY).ToList();
            DailyQuests = Quests.Where(x => x.Type == QuestType.DAILY).ToList();
            TimedQuests = Quests.Where(x => x.Type == QuestType.TIMED).ToList();
            StoryQuests = Quests.Where(x => x.Type == QuestType.STORY).ToList();
            WorldQuests = Quests.Where(x => x.Type == QuestType.WORLD).ToList();
            Plugin.LogInstance.LogInfo($"Loaded Quests into Category Buckets: OK");

            return true;
        }
        catch (Exception e)
        { 
            Plugin.LogInstance.LogError($"Error Load Database: {e.Message}");
            return false;
        }
    }

    public bool CreateDatabaseFiles()
    { 
        if(!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);
        if (!File.Exists(QuestGiverFile)) File.WriteAllText(QuestGiverFile, "[]");

        if (!File.Exists(QuestsFile)) 
        {
            List<QuestModel> template = new List<QuestModel>();

            QuestModel example = new QuestModel
                (
                    1,
                    "Example Quest",
                    "Kill 5 Forest Wolves",
                    QuestType.DAILY,
                    new QuestTarget
                        (
                            QuestTargetType.MobKill,
                            -1418430647,
                            5
                        ),
                    new Tuple<int, int> (1, 100),
                    [new Reward(-1169471531, 1)]
                );

            template.Add (example);
            var json = JsonSerializer.Serialize(template, new JsonSerializerOptions{ WriteIndented = true });

            File.WriteAllText(QuestsFile, json);
            Plugin.LogInstance.LogInfo($"Create Quest Database: OK");
        } 
        
        return true;
    }

    public bool GetQuestGiver(string giverName, out QuestGiverModel giver)
    { 
        giver = QuestGivers.FirstOrDefault(x => x.Name == giverName);

        if (giver == null)
        {
            return false;
        }

        return true;
    }

    public bool AddQuestGiver(string giverName, int giverGUID, QuestType type, bool immortal, bool canMove, bool autoRespawn)
    {
        if (GetQuestGiver(giverName, out QuestGiverModel giver))
        {
            Plugin.LogInstance.LogWarning($"Quest Giver with the name ({giverName}) already exists.");
            return false;
        }

        giver = new QuestGiverModel();
        giver.Name = giverName;
        giver.PrefabGUID = giverGUID;
        giver.QuestType = type;
        giver.config.Immortal = immortal;
        giver.config.CanMove = canMove;
        giver.config.AutoRespawn = autoRespawn;

        QuestGivers.Add(giver);
        SaveDatabase();
        return true;
    }

    public bool UpdateGiver(QuestGiverModel model)
    {
        if (GetQuestGiver(model.Name, out QuestGiverModel old))
        {
            old = model;
            SaveDatabase();
            return true;
        }

        return false;
    }

    public bool RemoveGiver(string giverName)
    {
        if (GetQuestGiver(giverName, out QuestGiverModel giver))
        {
            if (giver.config.IsEnabled)
            {
                Plugin.LogInstance.LogWarning($"Please disable giver before attempting removal");
                return false;
            }

            QuestGivers.Remove(giver);
            SaveDatabase();
            return true;
        }

        Plugin.LogInstance.LogWarning($"Unable to find quest giver with name: {giverName}");
        return false;
    }

    public bool GetQuest(int questID, out QuestModel quest)
    {
        if (Quests.Exists(x => x.ID == questID))
        {
            quest = Quests.FirstOrDefault(x => x.ID == questID);
            return true;
        }
        else
        {
            quest = null;
            return false;
        }
            
    }

    public static int[] ValidGiverIds { get; private set; } =
    [
            194933933,
            233171451,
            281572043,
            -1594911649,
            -1168705805,
            -375258845,
            -208499374,
            -1810631919,
            -1292194494,
            1631713257,
            345283594,
            -1990875761,
            1687896942,
            -915182578,
            739223277
    ];
}
