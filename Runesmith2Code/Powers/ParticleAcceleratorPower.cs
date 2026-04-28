#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Models;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class ParticleAcceleratorPower : Runesmith2Power, IModifyCharge, IAfterModifyingCharge, IModifyRunePassiveTriggerCount, IAfterModifyingRunePassiveTriggerCount
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public decimal ModifyCharge(Player player, decimal charge, ValueProp props, CardModel? cardSource)
    {
        if (player != Owner.Player && charge > 0)
        {
            return charge + Amount;
        }

        return charge;
    }

    public Task AfterModifyingCharge()
    {
        Flash();
        return Task.CompletedTask;
    }

    public int ModifyRunePassiveTriggerCounts(int triggerCount, Player player)
    {
        if (player != Owner.Player && triggerCount > 0)
        {
            return triggerCount + Amount;
        }

        return triggerCount;
    }

    public Task AfterModifyingRunePassiveTriggerCount()
    {
        Flash();
        return Task.CompletedTask;
    }
}