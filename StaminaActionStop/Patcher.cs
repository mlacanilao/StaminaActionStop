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
        
        [HarmonyPostfix]
        [HarmonyPatch(declaringType: typeof(AIAct), methodName: nameof(AIAct.Start))]
        public static void AIActStart(AIAct __instance)
        {
            AIActPatch.StartPostfix(__instance: __instance);
        }
    }
}