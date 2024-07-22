using Bloodstone.API;
using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using CrimsonQuest.DB.Models;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;

namespace CrimsonQuest.Utils;

internal class QuestService
{
    public void GetQuest(UserModel player, QuestProgressModel progress, QuestGiverModel giver)
    {
        if (giver.QuestType == QuestType.DAILY)
        {
            if (progress.CompletedDaily + progress.DailyQuests.Count >= Plugin.Settings.MAX_DAILY.Value) return;

            if (progress.DailyQuests.Count >= Plugin.Settings.ACTIVE_DAILY.Value ) return;

            if (giver.GetRandomQuest(player, progress, out QuestModel quest))
            {
                progress.DailyQuests.Add(new QuestSlot
                    (
                       quest, giver.Name
                    ));

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, player.Entity.Read<User>(),
                        $"You've accepted the <color=#ffc905>{quest.Name}</color>: {quest.Description}.");
            }
            else
            {
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, player.Entity.Read<User>(),
                        $"I have no quests to give you at this time...");
            }
        }

        if (giver.QuestType == QuestType.WEEKLY)
        {
            if (progress.CompletedWeekly + progress.WeeklyQuests.Count >= Plugin.Settings.MAX_WEEKLY.Value) return;

            if (progress.WeeklyQuests.Count >= Plugin.Settings.ACTIVE_WEEKLY.Value) return;

            if (giver.GetRandomQuest(player, progress, out QuestModel quest))
            {
                progress.WeeklyQuests.Add(new QuestSlot
                    (
                       quest, giver.Name
                    ));

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, player.Entity.Read<User>(),
                        $"You've accepted the <color=#ffc905>{quest.Name}</color>: {quest.Description}.");
            }
            else
            {
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, player.Entity.Read<User>(),
                        $"I have no quests to give you at this time...");
            }
        }

        CrimsonCore.PlayerData.UpdatePlayer(player, progress);
    }

    public void CompleteGiverQuests(UserModel player, QuestProgressModel progress, QuestGiverModel targetGiver)
    {
        foreach (QuestSlot slot in progress.WeeklyQuests)
        {
            if (slot.QuestGiverName != targetGiver.Name) continue;
            CompleteGiverQuest(slot, player, progress, targetGiver);
        }

        foreach (QuestSlot slot in progress.DailyQuests)
        {
            if (slot.QuestGiverName != targetGiver.Name) continue;
            CompleteGiverQuest(slot, player, progress, targetGiver);
        }

        CrimsonCore.PlayerData.UpdatePlayer(player, progress);
    }

    private void CompleteGiverQuest(QuestSlot slot, UserModel player, QuestProgressModel progress, QuestGiverModel targetGiver)
    {
        if (slot.QuestInProgress.Target.Type == QuestTargetType.MobKill)
        {
            if (slot.CheckForCompletion(out _, out _))
            {
                if (slot.isCompleted) return;
                foreach (Reward reward in slot.QuestInProgress.Rewards)
                {
                    UserSystem.TryAddInventoryItemOrDrop(player.Entity, new PrefabGUID(reward.GUID), reward.Quantity);
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, player.Entity.Read<User>(),
                        $"You have completed the quest <color=#ffc905>{slot.QuestInProgress.Name}</color>.");
                }

                slot.SetComplete();
            }
        }
    }

    public void UpdateMobQuestProgress(UserModel player, QuestProgressModel progress, PrefabGUID mob)
    {
        if (progress.WeeklyQuests.Count != 0)
        {
            for (int i = 0; i <= progress.WeeklyQuests.Count; i++)
            {
                if (progress.WeeklyQuests[i].QuestInProgress.Target.Type == QuestTargetType.MobKill)
                {
                    if (progress.WeeklyQuests[i].QuestInProgress.Target.GUID == mob.GuidHash)
                    {
                        QuestSlot value = progress.WeeklyQuests[i];
                        value.IncrementProgress();
                        progress.WeeklyQuests[i] = value;

                        CheckProgress(progress.WeeklyQuests[i], player);
                    }
                }
            }
        }

        if (progress.DailyQuests.Count != 0)
        {
            for (int i = 0; i <= progress.DailyQuests.Count; i++)
            {
                if (progress.DailyQuests[i].QuestInProgress.Target.Type == QuestTargetType.MobKill)
                {
                    if (progress.DailyQuests[i].QuestInProgress.Target.GUID == mob.GuidHash)
                    {
                        QuestSlot value = progress.DailyQuests[i];
                        value.IncrementProgress();
                        progress.DailyQuests[i] = value;

                        CheckProgress(progress.DailyQuests[i], player);
                    }
                }
            }
        }
        
        CrimsonCore.PlayerData.UpdatePlayer(player, progress);
    }

    public void UpdateVBloodQuestProgress(VBloodSystem __instance, NativeList<VBloodConsumed> deathEvents)
    {
        var entityManager = VWorld.Server.EntityManager;

        foreach (var event_vblood in deathEvents)
        {
            if (entityManager.HasComponent<PlayerCharacter>(event_vblood.Target))
            {
                var player = entityManager.GetComponentData<PlayerCharacter>(event_vblood.Target);

                var user = entityManager.GetComponentData<User>(player.UserEntity);

                var playerModel = GameData.Users.GetUserByCharacterName(user.CharacterName.ToString());

                if (CrimsonCore.PlayerData.GetProgressForUser(playerModel, out QuestProgressModel progress))
                {
                    if (progress.WeeklyQuests.Count != 0)
                    {
                        for (int i = 0; i <= progress.WeeklyQuests.Count; i++)
                        {
                            if (progress.WeeklyQuests[i].QuestInProgress.Target.Type == QuestTargetType.VBloodKill)
                            {
                                if (progress.WeeklyQuests[i].QuestInProgress.Target.GUID == event_vblood.Source._Value)
                                {
                                    QuestSlot value = progress.WeeklyQuests[i];
                                    value.IncrementProgress();
                                    progress.WeeklyQuests[i] = value;

                                    CheckProgress(progress.WeeklyQuests[i], playerModel);
                                }
                            }
                        }
                    }

                    if (progress.DailyQuests.Count != 0)
                    {
                        for (int i = 0; i <= progress.DailyQuests.Count; i++)
                        {
                            if (progress.DailyQuests[i].QuestInProgress.Target.Type == QuestTargetType.VBloodKill)
                            {
                                if (progress.DailyQuests[i].QuestInProgress.Target.GUID == event_vblood.Source._Value)
                                {
                                    QuestSlot value = progress.DailyQuests[i];
                                    value.IncrementProgress();
                                    progress.DailyQuests[i] = value;

                                    CheckProgress(progress.DailyQuests[i], playerModel);
                                }
                            }
                        }
                    }

                    CrimsonCore.PlayerData.UpdatePlayer(playerModel, progress);
                }
            }
        }
    }

    public void UpdatePVPKillQuestProgress(DeathEventListenerSystem __instance, NativeArray<DeathEvent> deathEvents)
    {
        var entityManager = VWorld.Server.EntityManager;

        foreach(var event_kill in deathEvents) 
        {
            if (entityManager.HasComponent<PlayerCharacter>(event_kill.Died))
            {
                if (entityManager.HasComponent<PlayerCharacter>(event_kill.Killer))
                {
                    var player = entityManager.GetComponentData<PlayerCharacter>(event_kill.Killer);

                    var user = entityManager.GetComponentData<User>(player.UserEntity);

                    var playerModel = GameData.Users.GetUserByCharacterName(user.CharacterName.ToString());

                    if (CrimsonCore.PlayerData.GetProgressForUser(playerModel, out QuestProgressModel progress)) 
                    {
                        if (progress.WeeklyQuests.Count != 0)
                        {
                            for (int i = 0; i <= progress.WeeklyQuests.Count; i++)
                            {
                                if (progress.WeeklyQuests[i].QuestInProgress.Target.Type == QuestTargetType.PlayerKill)
                                {
                                
                                        QuestSlot value = progress.WeeklyQuests[i];
                                        value.IncrementProgress();
                                        progress.WeeklyQuests[i] = value;

                                        CheckProgress(progress.WeeklyQuests[i], playerModel);
                                  
                                }
                            }
                        }

                        if (progress.DailyQuests.Count != 0)
                        {
                            for (int i = 0; i <= progress.DailyQuests.Count; i++)
                            {
                                if (progress.DailyQuests[i].QuestInProgress.Target.Type == QuestTargetType.PlayerKill)
                                {
                                        QuestSlot value = progress.DailyQuests[i];
                                        value.IncrementProgress();
                                        progress.DailyQuests[i] = value;

                                        CheckProgress(progress.DailyQuests[i], playerModel);
                                }
                            }
                        }

                        CrimsonCore.PlayerData.UpdatePlayer(playerModel, progress);
                    }
                }
            }
        }
    }

    private void CheckProgress(QuestSlot slot, UserModel player)
    {
        if (slot.CheckForCompletion(out int current, out int goal))
        {
            ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, player.Entity.Read<User>(),
                $"You have completed the quest <color=#ffc905>{slot.QuestInProgress.Name}</color>. Return to <color=#ffc905>{slot.QuestGiverName}</color>!");
        }
        else
        {
            ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, player.Entity.Read<User>(),
                $"<color=#ffc905>{slot.QuestInProgress.Name}</color> Progress: <color=#ffc905>{current}/{goal}</color>!");
        }
    }
}
