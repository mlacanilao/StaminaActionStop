using System;
using System.Runtime.CompilerServices;
using BepInEx;
using HarmonyLib;
using StaminaActionStop.Config;

namespace StaminaActionStop;

internal static class ModInfo
{
    internal const string Guid = "omegaplatinum.elin.staminaactionstop";
    internal const string Name = "Stamina Action Stop";
    internal const string Version = "2.0.0";
    internal const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
}

[BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
internal class StaminaActionStop : BaseUnityPlugin
{
    internal static StaminaActionStop? Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        StaminaActionStopConfig.LoadConfig(config: Config);
        Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: ModInfo.Guid);

        if (HasModOptionsPlugin() == false)
        {
            return;
        }

        try
        {
            UI.UIController.RegisterUI();
        }
        catch (Exception ex)
        {
            LogError(message: $"An error occurred during UI registration: {ex}");
        }
    }

    internal static void LogDebug(object message, [CallerMemberName] string caller = "")
    {
        Instance?.Logger.LogDebug(data: $"[{caller}] {message}");
    }

    internal static void LogInfo(object message)
    {
        Instance?.Logger.LogInfo(data: message);
    }

    internal static void LogError(object message)
    {
        Instance?.Logger.LogError(data: message);
    }

    private static bool HasModOptionsPlugin()
    {
        try
        {
            foreach (var obj in ModManager.ListPluginObject)
            {
                if (obj is not BaseUnityPlugin plugin)
                {
                    continue;
                }

                if (plugin.Info.Metadata.GUID == ModInfo.ModOptionsGuid)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            LogError(message: $"Error while checking for Mod Options: {ex}");
            return false;
        }
    }
}
