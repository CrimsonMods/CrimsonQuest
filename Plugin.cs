using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Bloodstone.API;
using Bloody.Core;
using Bloody.Core.API.v1;
using CrimsonQuest.Structs;
using CrimsonQuest.Utils;
using HarmonyLib;
using VampireCommandFramework;

namespace CrimsonQuest;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
[Bloodstone.API.Reloadable]
public class Plugin : BasePlugin, IRunOnInitialized
{
    private Harmony _harmony;
    public static ManualLogSource LogInstance { get; private set; }
    public static Settings Settings { get; private set; }
    public static SystemsCore SystemsCore { get; private set; }

    public override void Load()
    {
        LogInstance = Log;
        Settings = new(Config);
        Settings.InitConfig();

        if (!VWorld.IsServer) Log.LogWarning("This plugin is a server-only plugin.");
        CommandRegistry.RegisterAll();
    }

    public void OnGameInitialized()
    {
        if (VWorld.IsClient) return;

        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        CrimsonCore.InitializeAfterLoaded();
        SystemsCore = Core.SystemsCore;

        EventsHandlerSystem.OnDeathVBlood += CrimsonCore.Quest.UpdateVBloodQuestProgress;
        EventsHandlerSystem.OnDeath += CrimsonCore.Quest.UpdatePVPKillQuestProgress;

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }
}
