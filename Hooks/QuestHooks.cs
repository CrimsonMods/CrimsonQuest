using Bloodstone.API;
using Bloody.Core;
using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using CrimsonQuest.DB.Models;
using CrimsonQuest.Structs;
using CrimsonQuest.Utils;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Systems;
using Stunlock.Core;
using Unity.Collections;

namespace CrimsonQuest.Hooks;

[HarmonyPatch]
internal class QuestHooks
{
    
    [HarmonyPatch(typeof(OnKillSystem), nameof(OnKillSystem.OnUpdate))]
    [HarmonyPrefix]
    public static void OnUpdate_KillMobs(OnKillSystem __instance)
    {
        if (!Plugin.Settings.GetActiveSystem(Systems.ENABLE)) return;
        var _entities = __instance.__query_1438746367_0.ToEntityArray(Allocator.Temp);

        foreach (var _entity in _entities)
        {
            var _event = _entity.Read<DeathEvent>();

            if (VWorld.Server.EntityManager.HasComponent<PlayerCharacter>(_event.Died) ||
                !VWorld.Server.EntityManager.HasComponent<PrefabGUID>(_event.Died) ||
                VWorld.Server.EntityManager.HasComponent<VBloodUnit>(_event.Died) ||
                !VWorld.Server.EntityManager.TryGetComponentData<PlayerCharacter>(_event.Killer, out var _killer)) continue;

            PrefabGUID _mob = _event.Died.Read<PrefabGUID>();

            UserModel _player = GameData.Users.FromEntity(_killer.UserEntity);
            CrimsonCore.PlayerData.GetProgressForUser(_player, out QuestProgressModel progress);
            CrimsonCore.Quest.UpdateMobQuestProgress(_player, progress, _mob);
        }
    }
}
