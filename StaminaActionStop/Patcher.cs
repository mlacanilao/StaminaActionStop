using StaminaActionStop.Patches;
using HarmonyLib;

namespace StaminaActionStop;

internal static class Patcher
{
    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(StatsStamina), methodName: nameof(StatsStamina.Mod))]
    internal static void StatsStaminaModPrefix(StatsStamina __instance, out int __state)
    {
        __state = __instance.value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(StatsStamina), methodName: nameof(StatsStamina.Mod))]
    internal static void StatsStaminaMod(StatsStamina __instance, int a, int __state)
    {
        StatsStaminaPatch.ModPostfix(__instance: __instance, a: a, previousStaminaValue: __state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(AIAct), methodName: nameof(AIAct.Start))]
    internal static bool AIActStart(AIAct __instance)
    {
        return AIActPatch.StartPrefix(__instance: __instance);
    }
}
