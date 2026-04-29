#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class SyntheticBloodPower : Runesmith2Power
{
    private class Data
    {
        public CardModel? CardToModify;
        public int AmountWhenCardPlayed;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner) return amount;

        var elements = Owner.Player?.PlayerCombatState?.Elements() ?? new Elements(0);

        if (elements.Total <= 0 || Owner.Player == null) return amount;

        var remElements = elements;
        var remDamage = remElements.SubtractSequential((int)amount);
        var elemToLose = elements - remElements;
        RunesmithPlayerCmd.LoseElements(elemToLose, Owner.Player);
        return remDamage;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player) await PowerCmd.Decrement(this);
    }
}