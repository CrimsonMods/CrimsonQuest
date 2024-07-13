using Bloodstone.API;
using CrimsonQuest.DB.Models;
using System.Linq;
using Unity.Entities;
using Unity.Transforms;
using VampireCommandFramework;
using CrimsonQuest.Utils;

namespace CrimsonQuest.Commands;

[CommandGroup("crimsonquest", "cq")]
internal class QuestGiverCommands
{
    [Command("list", "l", "List of Quest Givers", adminOnly: true)]
    public void ListQuestGivers(ChatCommandContext ctx)
    {
        string message = $"Quest Givers:\n";

        if (CrimsonCore.QuestData.QuestGivers.Count == 0)
        {
            message += $"None exist... create some!";
        }
        else
        { 
            foreach(QuestGiverModel giver in CrimsonCore.QuestData.QuestGivers) 
            {
                message += $"{giver.Name} has {giver.QuestsToGive.Count} {giver.QuestType}\n";
            }
        }

        ctx.Reply(message);
    }

    [Command("create", usage: "<NameOfNPC> [PrefabGUID] [QuestType] [Immortal] [Move] [AutoRespawn]", description: "Creates a new Quest Giver", adminOnly: true)]
    public void CreateQuestGiver(ChatCommandContext ctx, string npcName, int npcPrefab = -1810631919, QuestType npcType = QuestType.DAILY, bool immortal = false, bool canMove = false, bool autoRespawn = true)
    {
        bool success = CrimsonCore.QuestData.AddQuestGiver(npcName, npcPrefab, npcType, immortal, canMove, autoRespawn);

        if (success)
        {
            ctx.Reply($"{npcName} created successfully");
        }
    }

    [Command("spawn", usage: "NameOfNPC", description: "Spawns a quest giver in your location", adminOnly: true)]
    public void Spawn(ChatCommandContext ctx, string giverName) 
    {
        if (CrimsonCore.QuestData.GetQuestGiver(giverName, out QuestGiverModel giver))
        {
            Entity user = ctx.Event.SenderUserEntity;
            var pos = VWorld.Server.EntityManager.GetComponentData<LocalToWorld>(user).Position;
            giver.SpawnWithLocation(user, pos);
            ctx.Reply($"Quest Giver {giverName} has spawned.");
        }
        else
        {
            ctx.Reply($"Unable to spawn {giverName}");
        }
    }

    [Command("add", usage: "QuestID", description: "Gives the quest to the Giver's list", adminOnly: true)]
    public void AddQuestToGiver(ChatCommandContext ctx, string giverName, int id)
    {
        if (CrimsonCore.QuestData.GetQuestGiver(giverName, out QuestGiverModel giver))
        {
            if (giver.AddQuest(id))
            {
                ctx.Reply($"Successfully added quest to {giverName}");
            }
            else
            {
                ctx.Reply($"Failed to find quest with id {id}");
            }
        }
    }
}
