using StaminaActionStop.Config;
using StaminaActionStop.Utils;

namespace StaminaActionStop.Patches
{
    internal static class AIActPatch
    {
        public static void StartPostfix(AIAct __instance)
        {
            bool enablePreActionCheck = StaminaActionStopConfig.enablePreActionCheck?.Value ?? false;

            if (enablePreActionCheck == false)
            {
                return;
            }
            
            var owner = __instance?.owner;
            if (owner == null || owner.IsPC == false)
            {
                return;
            }
            
            StaminaActionStopUtils.TryStopByThreshold(owner: owner, isFromStaminaPatch: true);
        }
    }
}