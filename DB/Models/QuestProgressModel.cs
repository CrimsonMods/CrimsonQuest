using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CrimsonQuest.DB.Models;

internal class QuestProgressModel
{ 
    public string character { get; set; } = string.Empty;
    public int CompletedDaily = 0;
    public int CompletedWeekly = 0;
    public List<QuestSlot> WeeklyQuests { get; set; } = new List<QuestSlot>();
    public List<QuestSlot> DailyQuests { get; set; } = new List<QuestSlot>();
    public QuestSlot TimedQuest { get; set; } = new QuestSlot();
    public QuestSlot StoryQuest { get; set; } = new QuestSlot();
    public List<int> CompletedStories { get; set; } = new List<int>();

    [JsonConstructor]
    public QuestProgressModel()
    { 
    
    }

    public QuestProgressModel(string characterName)
    { 
        character = characterName;
        WeeklyQuests = new List<QuestSlot>();
        DailyQuests = new List<QuestSlot>();
        TimedQuest = new QuestSlot();
        StoryQuest = new QuestSlot();
        CompletedStories = new List<int>();
    }
}

public class QuestSlot
{
    public QuestModel QuestInProgress { get; set; }
    public int TargetProgress { get; set; }
    public string QuestGiverName { get; set; }
    public bool isCompleted { get; set; }

    [JsonConstructor]
    public QuestSlot() { }

    public QuestSlot(QuestModel quest, string giverName)
    {
        QuestInProgress = quest;
        TargetProgress = 0;
        QuestGiverName = giverName;
        isCompleted = false;
    }

    public void IncrementProgress()
    {
        TargetProgress++;
    }

    public bool CheckForCompletion(out int current, out int goal)
    {
        current = TargetProgress;
        goal = QuestInProgress.Target.Amount;
        if (current >= goal) return true;
        return false;
    }

    public void SetComplete()
    {
        isCompleted = true;
    }
}
