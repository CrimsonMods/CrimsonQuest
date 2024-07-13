# CrimsonHunt
`Server side only` mod for a questing system in V Rising

WARNING: This is not a simple mod to configure and honestly it is quite tedious. I eventually plan to create a standalone system (JSONRising) that will allow creating quests easily; as well jsons for other Crimson mods.
NOTE: This mod is not finished. There is a lot of functionality in quests that does not work. 
Currently the following quest types and goals are implemented: Daily & Weekly Quests targetting Mob, VBlood, and PVP Kills. 

With CrimsonQuest you can create daily and weekly quests for players for a wide varity of objectives. Have they kill specific mobs, v bloods, each other, factions, gather materials, or even story quests to give your world a plotline!

Planned Expansions of the System:
- Story Quests: One time quests that can provide plot, teach mechanics, or even introduce other mods to players.
- World Quests: These are quests of which every player contributes to progress, good for world bosses
- Timed Quests: These are your typical MMO style quest that triggers when a player is doing something specific or goes somewhere

- Faction Objective: Setting quests to target specific factions of mobs
- Gather Objective: Setting quests to retrieve a specified amount of a item
- Mod Objective: Setting quests to perform an action that is part of another mod

## Installation
* Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/)
* Install [Bloodstone](https://github.com/decaprime/Bloodstone/releases/tag/v0.2.1)
* Install [Bloody.Core](https://thunderstore.io/c/v-rising/p/Trodi/BloodyCore/)
* Extract _CrimsonQuest_ into _(VRising server folder)/BepInEx/plugins_

## Configurable Values
```ini
[Config]

## Enable or disable the mod
# Setting type: Boolean
# Default value: true
EnableMod = true

## The max amount of daily quests a player can complete in one day
# Setting type: int
# Default value: 5
MaxDailies = 5

## The max amount of weekly quests a player can complete in a week
# Setting type: int
# Default value: 3
MaxWeeklies = 3

## The day weeklies reset, 0 - Sunday, 1 - Monday, and so on
# Setting type: int
# Default value: 1
ResetDay = 1

## The hour on which Daily & Weekly resets in 24-hour time (0-23)
# Setting type: int
# Default value: 0
ResetTime = 0
```

The json structure for the quest_list.json is not very straight-forward. I will try and present the valid values you can use and explain the structure itself instead.

```QuestModel
    int ID // this is an id for the mod, start at 0 or 1 and have them all unique
    string Name // This is the name of the quest that players will see
    string Description // A description of the quest
    QuestType Type // Reference the QuestType object below
    QuestTarget Target // Reference the QuestTarget object below
    Tuple<int, int> MinMaxLevel // This is two numbers to be set as valid levels. This quest will only be giving to players between these.
    List<Reward> Rewards // This is a list of Reward that will be giving to the player on completion. Reference Reward object.
```

```QuestType
    DAILY = 0, 
    WEEKLY = 1,
    TIMED = 2, // not yet supported
    WORLD = 3, // not yet supported
    STORY = 4 // supported, but not to the point I'd like
```

```QuestTarget
    QuestTargetType Type // reference QuestTargetType below
    int GUID // the ItemHash of the target
    int Amount // how many of the target the player must accomplish
```

```QuestTargetType
    ItemFetch = 0, // not yet supported
    MobKill = 1,
    PlayerKill = 2,
    VBloodKill = 3
```

```Reward
    int GUID // ItemHash for this reward
    int Quantity // how many of the item to give as reward
```

## Support

Want to support my V Rising Mod development? 

Donations Accepted with [Ko-Fi](https://ko-fi.com/skytech6)

Or buy/play my games! 

[Train Your Minibot](https://store.steampowered.com/app/713740/Train_Your_Minibot/) 

[Boring Movies](https://store.steampowered.com/app/1792500/Boring_Movies/)