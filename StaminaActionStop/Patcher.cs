using StaminaActionStop.Patches;
using HarmonyLib;

namespace StaminaActionStop
{
    public class Patcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(declaringType: typeof(StatsStamina), methodName: nameof(StatsStamina.Mod))]
        public static void StatsStaminaMod(StatsStamina __instance)
        {
            StatsStaminaPatch.ModPostfix(__instance: __instance);
        }
    }
}