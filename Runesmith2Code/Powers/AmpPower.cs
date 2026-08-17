#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class AmpPower : Runesmith2Power, IModifyPotencyAdditive, IAfterModifyingPotency
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public decimal ModifyPotencyAdditive(Player player, decimal amount, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource != null)
        {
            if (cardSource.Owner.Creature != Owner) return 0m;
        }
        else if (Owner.Player != player)
        {
            return 0m;
        }

        if (!props.IsPoweredCardOrMonsterMoveBlock()) return 0m;
        return Amount;
    }

    public async Task AfterModifyingPotency()
    {
        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -Amount, null, null);
    }
}