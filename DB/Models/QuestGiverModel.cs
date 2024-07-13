using Bloodstone.API;
using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.Helper.v1;
using Bloody.Core.Models.v1;
using CrimsonQuest.Utils;
using ProjectM;
using ProjectM.Network;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace CrimsonQuest.DB.Models;

internal class QuestGiverModel
{
    private Entity icontEntity;

    public string Name { get; set; } = string.Empty;
    public int PrefabGUID { get; set; }
    public List<int> QuestsToGive { get; set; } = new List<int>();
    public QuestType QuestType { get; set; }
    public Entity giverEntity { get; set; } = new();
    public ConfigQuestGiverModel config { get; set; } = new();

    public bool SpawnWithLocation(Entity sender, float3 pos)
    {
        SpawnSystem.SpawnUnitWithCallback(sender, new Stunlock.Core.PrefabGUID(PrefabGUID), new(pos.x, pos.z), -1, (Entity e) =>
        {
            giverEntity = e;
            config.X = pos.x;
            config.Z = pos.z;
            config.IsEnabled = true;


            ModifyGiver(sender, e);
        });

        AddIcon(giverEntity);

        return true;
    }

    public bool AddQuest(int id)
    {
        if (CrimsonCore.QuestData.GetQuest(id, out QuestModel quest))
        {
            if (quest == null)
            {
                return false;
            }

            if (quest.Type == QuestType)
            {
                QuestsToGive.Add(id);
                return true;
            }
        }
        return false;
    }

    public void ModifyGiver(Entity user, Entity giver)
    {
        if (config.Immortal)
        {
            bool _immortal = MakeNPCImmortal(user, giver);
            Plugin.LogInstance.LogDebug($"NPC Immortal: {_immortal}");
        }

        if (!config.CanMove)
        {
            bool _dontMove = MakeNPCNotMove(user, giver);
            Plugin.LogInstance.LogDebug($"NPC Not Move {_dontMove}");
        }

        RenameGiver(giver, Name);
    }

    private void RenameGiver(Entity giver, string newName)
    {
        if (!giver.Has<NameableInteractable>())
        {
            giver.Add<NameableInteractable>();
        }

        NameableInteractable nameable = giver.Read<NameableInteractable>();
        nameable.Name = new FixedString64Bytes(newName);
        giver.Write(nameable);

        CrimsonCore.QuestData.UpdateGiver(this);
    }

    public bool MakeNPCNotMove(Entity user, Entity giver)
    {
        var buff = Prefabs.Buff_BloodQuality_T01_OLD;
        var _des = VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>();
        var _event = new ApplyBuffDebugEvent() { BuffPrefabGUID = buff };
        var _from = new FromCharacter()
        {
            User = user,
            Character = giver
        };

        _des.ApplyBuff(_from, _event);
        if (BuffUtility.TryGetBuff(VWorld.Server.EntityManager, giver, buff, out var _buffEntity))
        {
            if (!_buffEntity.Has<BuffModificationFlagData>())
            {
                _buffEntity.Add<BuffModificationFlagData>();
            }

            var _buffModificationFlagData = _buffEntity.Read<BuffModificationFlagData>();
            _buffModificationFlagData.ModificationTypes = (long)BuffModificationTypes.MovementImpair;
            _buffEntity.Write(_buffModificationFlagData);

            return true;
        }
        return false;
    }

    public bool MakeNPCImmortal(Entity user, Entity giver)
    {
        var buff = Prefabs.Buff_Manticore_ImmaterialHomePos;
        var _des = VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>();
        var _event = new ApplyBuffDebugEvent() { BuffPrefabGUID = buff };
        var _from = new FromCharacter()
        {
            User = user,
            Character = giver
        };

        _des.ApplyBuff(_from, _event);
        if (BuffUtility.TryGetBuff(VWorld.Server.EntityManager, giver, buff, out var _buffEntity))
        {
            _buffEntity.Add<Buff_Persists_Through_Death>();
            if (_buffEntity.Has<LifeTime>())
            {
                var _time = _buffEntity.Read<LifeTime>();
                _time.Duration = -1;
                _time.EndAction = LifeTimeEndAction.None;
                _buffEntity.Write(_time);
            }

            if (_buffEntity.Has<RemoveBuffOnGameplayEvent>())
                _buffEntity.Remove<RemoveBuffOnGameplayEvent>();

            if (_buffEntity.Has<RemoveBuffOnGameplayEventEntry>())
                _buffEntity.Remove<RemoveBuffOnGameplayEventEntry>();

            return true;
        }
        return false;
    }

    public float3 GetPosition()
    {
        return new float3(config.X, 0, config.Z);
    }

    public void AddIcon(Entity giver)
    {
        SpawnSystem.SpawnUnitWithCallback(giver, Prefabs.MapIcon_POI_Discover_Merchant, new float2(config.X, config.Z), -1, (Entity e) => {
            icontEntity = e;
            e.Add<MapIconData>();
            e.Add<MapIconTargetEntity>();
            var mapIconTargetEntity = e.Read<MapIconTargetEntity>();
            mapIconTargetEntity.TargetEntity = NetworkedEntity.ServerEntity(giver);
            mapIconTargetEntity.TargetNetworkId = giver.Read<NetworkId>();
            e.Write(mapIconTargetEntity);
            e.Add<NameableInteractable>();
            NameableInteractable _nameableInteractable = e.Read<NameableInteractable>();
            _nameableInteractable.Name = new FixedString64Bytes(Name + "_icon");
            e.Write(_nameableInteractable);
        });
    }

    private bool GetIcon(string nameMerchant)
    {
        var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, MapIconData>(EntityQueryOptions.IncludeDisabledEntities);
        foreach (var entity in entities)
        {
            NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
            if (_nameableInteractable.Name.Value == nameMerchant + "_icon")
            {
                icontEntity = entity;
                entities.Dispose();
                return true;
            }
        }
        entities.Dispose();
        return false;
    }

    public bool GetEntity(string nameGiver)
    {
        var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, TradeCost>(EntityQueryOptions.IncludeDisabledEntities);
        foreach (var entity in entities)
        {
            NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
            if (_nameableInteractable.Name.Value == nameGiver)
            {
                giverEntity = entity;
                entities.Dispose();
                return true;
            }
        }
        entities.Dispose();
        return false;
    }

    public bool GetRandomQuest(UserModel user, QuestProgressModel progress, out QuestModel quest)
    {
        quest = null;
        List<QuestModel> TargetType = new List<QuestModel>();

        List<int> InProgressQuests = new List<int>();
        foreach (QuestSlot progression in progress.DailyQuests.Concat(progress.WeeklyQuests))
        {
            InProgressQuests.Add(progression.QuestInProgress.ID);
        }

        switch (QuestType)
        {
            case QuestType.DAILY:
                TargetType = CrimsonCore.QuestData.DailyQuests.Where(x => x.Type == QuestType.DAILY).ToList();
                break;
            case QuestType.WEEKLY:
                TargetType = CrimsonCore.QuestData.WeeklyQuests.Where(x => x.Type == QuestType.WEEKLY).ToList();
                break;
        }

        TargetType = TargetType.Where(x => !InProgressQuests.Contains(x.ID)).ToList();
        TargetType = TargetType.Where(x => x.MinMaxLevel.Item1 <= user.Equipment.Level).ToList();
        TargetType = TargetType.Where(x => user.Equipment.Level <= x.MinMaxLevel.Item2).ToList();
                
        if (TargetType.Count == 0)
        {
            Plugin.LogInstance.LogDebug($"{user.CharacterName} requested quest of type {QuestType.ToString()} with item level {user.Equipment.Level} and found 0 results.");
            return false;
        }
        else
        {
            quest = TargetType[UnityEngine.Random.Range(0, TargetType.Count)];
            return true;
        }
    }

}
