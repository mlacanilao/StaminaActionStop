using StaminaActionStop.Config;
using StaminaActionStop.Utils;

namespace StaminaActionStop.Patches;

internal static class StatsStaminaPatch
{
    public static void ModPostfix(StatsStamina __instance, int a, int previousStaminaValue)
    {
        if (a >= 0)
        {
            return;
        }

        if (__instance.value >= previousStaminaValue)
        {
            return;
        }

        if (StaminaActionStopConfig.EnableStaminaCheck.Value == false)
        {
            return;
        }

        var owner = __instance.Owner;
        if (owner == null || owner.IsPC == false)
        {
            return;
        }

        StaminaActionStopUtils.TryStopRunningAction(owner: owner);
    }
}
