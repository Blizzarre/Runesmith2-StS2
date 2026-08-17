#region

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class ParticleAcceleratorPower : Runesmith2Power, IModifyCharge, IAfterModifyingCharge,
    IModifyRunePassiveTriggerCount, IAfterModifyingRunePassiveTriggerCount, IHasSecondAmount
{
    private const string TriggerCountKey = "TriggerCount";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(TriggerCountKey, 0)
    ];

    public decimal ModifyCharge(Player player, decimal charge, ValueProp props, CardModel? cardSource)
    {
        if (player == Owner.Player && charge > 0) return charge + Amount;

        return charge;
    }

    public Task AfterModifyingCharge()
    {
        Flash();
        return Task.CompletedTask;
    }

    public int ModifyRunePassiveTriggerCounts(int triggerCount, Player player)
    {
        if (player == Owner.Player && (player.PlayerCombatState?.GetRuneQueue()?.HasAny() ?? false) &&
            triggerCount >= 0)
            return triggerCount + DynamicVars[TriggerCountKey].IntValue;

        return triggerCount;
    }

    public Task AfterModifyingRunePassiveTriggerCount()
    {
        Flash();
        return Task.CompletedTask;
    }

    public void IncrementTriggerCount(int amount)
    {
        AssertMutable();
        DynamicVars[TriggerCountKey].BaseValue += amount;
    }

    public string GetSecondAmount()
    {
        return DynamicVars[TriggerCountKey].IntValue.ToString();
    }
}