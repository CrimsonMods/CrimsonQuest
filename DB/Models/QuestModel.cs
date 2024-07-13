using Bloody.Core.Models.v1.Internals;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimsonQuest.DB.Models;

public class QuestModel
{ 
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public QuestType Type { get; set; }
    public QuestTarget Target { get; set; }
    public Tuple<int, int> MinMaxLevel { get; set; }
    public List<Reward> Rewards { get; set; }

    public QuestModel(int iD, string name, string description, QuestType type, QuestTarget target, Tuple<int, int> minMaxLevel, List<Reward> rewards)
    {
        ID = iD;
        Name = name;
        Description = description;
        Type = type;
        Target = target;
        MinMaxLevel = minMaxLevel;
        Rewards = rewards;
    }
}

public class QuestTarget
{
    public QuestTargetType Type { get; set; }
    public int GUID { get; set; }
    public int Amount { get; set; }

    public QuestTarget(QuestTargetType type, int guid, int amount)
    { 
        Type = type;
        GUID = guid;
        Amount = amount;
    }
}

public class Reward
{ 
    public int GUID { get; set; }
    public int Quantity { get; set; }

    public Reward(int guid, int quantity)
    {
        GUID = guid;
        Quantity = quantity;
    }

}

public enum QuestTargetType
{ 
    ItemFetch,
    MobKill,
    PlayerKill,
    VBloodKill
}

public enum QuestType
{ 
    DAILY,
    WEEKLY,
    TIMED,
    WORLD,
    STORY
}
