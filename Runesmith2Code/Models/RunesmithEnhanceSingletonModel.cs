using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Extensions;

namespace Runesmith2.Runesmith2Code.Models;

public class RunesmithEnhanceSingletonModel() : CustomSingletonModel(true, false), IRunesmithModel
{
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (cardSource == null)
        {
            return 1;
        }

        if (!props.IsPoweredAttack_())
        {
            return 1;
        }

        if (!cardSource.IsEnhanced())
        {
            return 1;
        }

        return 1 + cardSource.GetEnhanceMultiplier();
    }

    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource == null)
        {
            return 1;
        }

        if (!props.IsPoweredCardOrMonsterMoveBlock_())
        {
            return 1;
        }

        if (!cardSource.IsEnhanced())
        {
            return 1;
        }

        return 1 + cardSource.GetEnhanceMultiplier();
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsStasis()) return Task.CompletedTask;
        if (cardPlay.IsLastInSeries && cardPlay.Card.IsEnhanced())
        {
            cardPlay.Card.ClearEnhance();
        }

        return Task.CompletedTask;
    }

    public decimal ModifyPotencyMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource == null)
        {
            return 1;
        }

        if (!props.IsPoweredCardOrMonsterMoveBlock_())
        {
            return 1;
        }

        if (!cardSource.IsEnhanced())
        {
            return 1;
        }

        return 1 + cardSource.GetEnhanceMultiplier();
    }
}