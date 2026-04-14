using StaminaActionStop.Config;
using StaminaActionStop.Utils;

namespace StaminaActionStop.Patches;

internal static class AIActPatch
{
    public static bool StartPrefix(AIAct __instance)
    {
        if (StaminaActionStopConfig.EnablePreActionCheck.Value == false)
        {
            return true;
        }

        bool wasStopped = StaminaActionStopUtils.TryStopBeforeStart(act: __instance);
        return wasStopped == false;
    }
}
