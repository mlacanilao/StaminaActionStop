using StaminaActionStop.Config;

namespace StaminaActionStop.Utils
{
    internal static class StaminaActionStopUtils
    {
        internal static void TryStopByThreshold(
            Chara owner,
            bool isFromStaminaPatch)
        {
            if (owner == null)
            {
                return;
            }

            var ai = owner.ai;
            if (ai == null ||
                ai.IsRunning == false ||
                ai is AI_Goto ||
                ai is GoalManualMove ||
                ai is AI_Eat ||
                ai is AI_Meditate ||
                ai is TaskDump)
            {
                return;
            }

            var stamina = owner.stamina;
            var hp = owner.hp;
            var mana = owner.mana.value;
            var hunger = owner.hunger;
            var sleepiness = owner.sleepiness;

            if (isFromStaminaPatch == true &&
                ai is AI_UseCrafter crafterAI &&
                crafterAI.num >= 2)
            {
                return;
            }

            if (StaminaActionStopConfig.enableThresholdValue?.Value == true &&
                stamina.value <= StaminaActionStopConfig.staminaThresholdValue?.Value)
            {
                Msg.SetColor(color: Msg.colors.TalkGod);
                Msg.Say(idLang: "textEnc2", ref1: ModInfo.Name, ref2: "stamina".lang());
                ai.Current.TryCancel(c: null);
                ai.TryCancel(c: null);
                return;
            }

            if (StaminaActionStopConfig.enableThresholdPhase?.Value == true &&
                stamina.GetPhase() <= StaminaActionStopConfig.staminaThresholdPhase?.Value)
            {
                Msg.SetColor(color: Msg.colors.TalkGod);
                Msg.Say(idLang: "textEnc2", ref1: ModInfo.Name, ref2: "stamina".lang());
                ai.Current.TryCancel(c: null);
                ai.TryCancel(c: null);
                return;
            }
            
            if (StaminaActionStopConfig.enableHpThresholdValue?.Value == true &&
                stamina.value < 0 &&
                (hp <= StaminaActionStopConfig.hpThresholdValue?.Value || hp + stamina.value <= 0))
            {
                Msg.SetColor(color: Msg.colors.TalkGod);
                Msg.Say(idLang: "textEnc2", ref1: ModInfo.Name, ref2: "health".lang());
                ai.Current.TryCancel(c: null);
                ai.TryCancel(c: null);
                return;
            }
            
            if (StaminaActionStopConfig.enableManaThresholdValue?.Value == true &&
                stamina.value <= 0 &&
                hp <= 0 &&
                (mana <= StaminaActionStopConfig.manaThresholdValue?.Value ||
                 mana + stamina.value <= 0))
            {
                Msg.SetColor(color: Msg.colors.TalkGod);
                Msg.Say(idLang: "textEnc2", ref1: ModInfo.Name, ref2: "cMana".lang());
                ai.Current.TryCancel(c: null);
                ai.TryCancel(c: null);
                return;
            }
        }
    }
}