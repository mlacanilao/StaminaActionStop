using StaminaActionStop.Config;

namespace StaminaActionStop.Patches
{
    public static class StatsStaminaPatch
    {
        public static void ModPostfix(StatsStamina __instance)
        {
            if (BaseStats.CC.IsPC)
            {
                var currentAI = BaseStats.CC.ai;
                
                if (currentAI is AI_UseCrafter crafterAI)
                {
                    if (crafterAI.num >= 2) return;
                }

                if (StaminaActionStopConfig.enableThresholdValue.Value &&
                    __instance.value <= StaminaActionStopConfig.staminaThresholdValue.Value)
                {
                    if (currentAI != null && currentAI.IsRunning)
                    {
                        currentAI.TryCancel(c: null);
                    }
                    return;
                }

                if (StaminaActionStopConfig.enableThresholdPhase.Value &&
                    __instance.GetPhase() <= StaminaActionStopConfig.staminaThresholdPhase.Value)
                {
                    if (currentAI != null && currentAI.IsRunning)
                    {
                        currentAI.TryCancel(c: null);
                    }
                }
            }
        }
    }
}