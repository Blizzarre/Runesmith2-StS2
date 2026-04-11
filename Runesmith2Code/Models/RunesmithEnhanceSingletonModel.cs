using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Extensions;

namespace Runesmith2.Runesmith2Code.Models;

public class RunesmithEnhanceSingletonModel() : CustomSingletonModel(true, false)
{
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
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
        
        // modify enhance multiplier here

        return 1 + 0.5m * cardSource.GetEnhance();
    }

    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
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

        return 1 + 0.5m * cardSource.GetEnhance();
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
}