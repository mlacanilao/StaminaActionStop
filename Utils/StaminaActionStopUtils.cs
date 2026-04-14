using StaminaActionStop.Config;

namespace StaminaActionStop.Utils;

internal static class StaminaActionStopUtils
{
    private enum StopReason
    {
        None,
        Stamina,
        Health,
        Mana
    }

    internal static bool TryStopBeforeStart(AIAct act)
    {
        var owner = act?.owner;
        if (owner == null ||
            owner.IsPC == false ||
            CanUsePreStartCheck(owner: owner, act: act) == false)
        {
            return false;
        }

        if (ShouldStopByThreshold(
                owner: owner,
                activeAct: act,
                isFromStaminaPatch: false,
                stopReason: out StopReason stopReason) == false)
        {
            return false;
        }

        StopBeforeStart(act: act, stopReason: stopReason);
        return true;
    }

    private static bool CanUsePreStartCheck(Chara owner, AIAct act)
    {
        AIAct? rootAct = owner.ai;
        if (rootAct == null)
        {
            return false;
        }

        if (ReferenceEquals(objA: rootAct, objB: act))
        {
            return true;
        }

        return rootAct is AI_UseCrafter;
    }

    internal static bool TryStopRunningAction(Chara owner)
    {
        if (owner == null ||
            owner.IsPC == false ||
            owner.ai == null)
        {
            return false;
        }

        AIAct rootAct = owner.ai;
        AIAct activeAct = rootAct.Current;
        if (ShouldStopByThreshold(
                owner: owner,
                activeAct: activeAct,
                isFromStaminaPatch: true,
                stopReason: out StopReason stopReason) == false)
        {
            return false;
        }

        StopStartedAction(rootAct: rootAct, activeAct: activeAct, stopReason: stopReason);
        return true;
    }

    private static bool ShouldStopByThreshold(
        Chara owner,
        AIAct activeAct,
        bool isFromStaminaPatch,
        out StopReason stopReason)
    {
        stopReason = StopReason.None;

        if (activeAct == null ||
            activeAct.IsRunning == false)
        {
            return false;
        }

        if (IsExcludedActionType(activeAct: activeAct))
        {
            return false;
        }

        if (isFromStaminaPatch == true &&
            activeAct is AI_UseCrafter crafterAI &&
            crafterAI.num >= 2)
        {
            return false;
        }

        var stamina = owner.stamina;
        var hp = owner.hp;
        var mana = owner.mana.value;

        if (StaminaActionStopConfig.EnableThresholdValue.Value &&
            stamina.value <= StaminaActionStopConfig.StaminaThresholdValue.Value)
        {
            stopReason = StopReason.Stamina;
            return true;
        }

        if (StaminaActionStopConfig.EnableThresholdPhase.Value &&
            stamina.GetPhase() <= StaminaActionStopConfig.StaminaThresholdPhase.Value)
        {
            stopReason = StopReason.Stamina;
            return true;
        }

        if (StaminaActionStopConfig.EnableHpThresholdValue.Value &&
            stamina.value < 0 &&
            (hp <= StaminaActionStopConfig.HpThresholdValue.Value || hp + stamina.value <= 0))
        {
            stopReason = StopReason.Health;
            return true;
        }

        if (StaminaActionStopConfig.EnableManaThresholdValue.Value &&
            owner.Evalue(ele: FEAT.featManaMeat) > 0 &&
            stamina.value < 0 &&
            hp <= 0 &&
            mana <= StaminaActionStopConfig.ManaThresholdValue.Value)
        {
            stopReason = StopReason.Mana;
            return true;
        }

        return false;
    }

    private static bool IsExcludedActionType(AIAct activeAct)
    {
        return activeAct is AI_Goto ||
               activeAct is GoalManualMove ||
               activeAct is AI_Eat ||
               activeAct is AI_Meditate ||
               activeAct is TaskDump;
    }

    private static void StopBeforeStart(AIAct act, StopReason stopReason)
    {
        ShowStopMessage(stopReason: stopReason);
        act.Cancel();
    }

    private static void StopStartedAction(AIAct rootAct, AIAct activeAct, StopReason stopReason)
    {
        ShowStopMessage(stopReason: stopReason);
        activeAct.TryCancel(c: null);
        rootAct.TryCancel(c: null);
    }

    private static void ShowStopMessage(StopReason stopReason)
    {
        Msg.SetColor(color: Msg.colors.TalkGod);
        Msg.Say(idLang: "textEnc2", ref1: ModInfo.Name, ref2: GetStopResource(stopReason: stopReason));
    }

    private static string GetStopResource(StopReason stopReason)
    {
        return stopReason switch
        {
            StopReason.Stamina => "stamina".lang(),
            StopReason.Health => "health".lang(),
            StopReason.Mana => "cMana".lang(),
            _ => string.Empty
        };
    }
}
