using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using CrimsonQuest.DB.Models;
using CrimsonQuest.Structs;
using CrimsonQuest.Utils;
using VampireCommandFramework;

namespace CrimsonQuest.Commands;

[CommandGroup("crimsonquest", "cq")]
internal class QuestsCommands
{
    [Command("dailies", "daily", description: "Shows your current Daily Quests")]
    public void ListDailyQuests(ChatCommandContext ctx)
    {
        var entity = ctx.Event.SenderUserEntity;
        UserModel user = GameData.Users.FromEntity(entity);

        string message = $"Active Daily Quests:\n";

        if (CrimsonCore.PlayerData.GetProgressForUser(user, out QuestProgressModel progress))
        {
            if (progress.DailyQuests.Count == 0 || progress.DailyQuests == null)
            {
                message += "None";
            }
            else
            {
                foreach (QuestSlot slot in progress.DailyQuests)
                {
                    message += $"{slot.QuestInProgress.Description}\n";
                }
            }

            ctx.Reply(message);
        }
    }

    [Command("get", "accept", description: "Accepts a quest from the closest giver")]
    public void GetQuest(ChatCommandContext ctx)
    {
        var entity = ctx.Event.SenderUserEntity;
        UserModel player = GameData.Users.FromEntity(entity);

        foreach (QuestGiverModel _npc in CrimsonCore.QuestData.QuestGivers)
        {
            QuestArea _area = QuestArea.FromPos(_npc.GetPosition(), 5.0F);
            if (!_area.InQuestArea(player.Character.Entity))
            {
                Plugin.LogInstance.LogDebug($"{player.CharacterName} is not within 5m of {_npc.Name}");
                continue;
            }

            CrimsonCore.PlayerData.GetProgressForUser(player, out QuestProgressModel progress);

            CrimsonCore.Quest.GetQuest(player, progress, _npc);
        }
    }

    [Command("complete", description: "Turns in a completed quest to the nearest giver")]
    public void GiveQuest(ChatCommandContext ctx)
    {
        var entity = ctx.Event.SenderUserEntity;
        UserModel player = GameData.Users.FromEntity(entity);

        foreach (QuestGiverModel _npc in CrimsonCore.QuestData.QuestGivers)
        {
            QuestArea _area = QuestArea.FromPos(_npc.GetPosition(), 5.0F);
            if (!_area.InQuestArea(player.Character.Entity))
            {
                Plugin.LogInstance.LogDebug($"{player.CharacterName} is not within 5m of {_npc.Name}");
                continue;
            }

            CrimsonCore.PlayerData.GetProgressForUser(player, out QuestProgressModel progress);

            CrimsonCore.Quest.CompleteGiverQuests(player, progress, _npc);
        }
    }
}
