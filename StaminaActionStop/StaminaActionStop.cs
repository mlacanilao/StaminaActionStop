using BepInEx;
using HarmonyLib;
using StaminaActionStop.Config;
using UnityEngine;

namespace StaminaActionStop
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.staminaactionstop";
        internal const string Name = "Stamina Action Stop";
        internal const string Version = "1.0.1.0";
        internal const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class StaminaActionStop : BaseUnityPlugin
    {
        private void Start()
        {
            StaminaActionStopConfig.LoadConfig(config: Config);
            
            Harmony.CreateAndPatchAll(type: typeof(Patcher));
            
            try
            {
                UI.UIController.RegisterUI();
            }
            catch (System.IO.FileNotFoundException)
            {
                Debug.LogWarning("[Stamina Action Stop] Mod Options mod is not installed/enabled. Skipping UI registration.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Stamina Action Stop] An unexpected error occurred: {ex.Message}");
            }
        }
    }
}