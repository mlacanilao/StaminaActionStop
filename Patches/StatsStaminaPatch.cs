using StaminaActionStop.Config;

namespace StaminaActionStop.Patches
{
    public static class StatsStaminaPatch
    {
        public static void ModPostfix(StatsStamina __instance)
        {
            if (BaseStats.CC?.IsPC == false)
            {
                return;
            }
            
            var currentAI = BaseStats.CC?.ai;

            switch (currentAI)
            {
                case null:
                case AI_UseCrafter crafterAI when crafterAI.num >= 2:
                    return;
            }

            if (StaminaActionStopConfig.enableThresholdValue.Value &&
                __instance.value <= StaminaActionStopConfig.staminaThresholdValue?.Value)
            {
                if (currentAI.IsRunning)
                {
                    currentAI.TryCancel(c: null);
                }
                return;
            }

            if (StaminaActionStopConfig.enableThresholdPhase.Value &&
                __instance.GetPhase() <= StaminaActionStopConfig.staminaThresholdPhase?.Value)
            {
                if (currentAI.IsRunning)
                {
                    currentAI.TryCancel(c: null);
                }
            }
        }
    }
}