#region

using System.Diagnostics.CodeAnalysis;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class IceColdPower : Runesmith2Power
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DisplayVar<IceColdPower>("Decrement", pow => (pow.Amount / 2).ToString())];

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Player == null) return;
        var card = cardPlay.Card;
        if (card.IsUpgradable)
            CardCmd.Upgrade(card);
        else if (card.CanEnhance()) await RunesmithCardCmd.Enhance(choiceContext, Owner.Player, card, cardPlay, 1);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (Owner != dealer) return 0;

        if (!props.IsPoweredAttack()) return 0;

        return -Amount;
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side) await PowerCmd.ModifyAmount(choiceContext, this, -(Amount / 2), null, null);
    }
}