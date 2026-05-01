#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Models;

public class RunesmithEnhanceSingletonModel() : CustomSingletonModel(true, false), IModifyPotencyMultiplicative
{
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (cardSource == null) return 1;

        if (!props.IsPoweredAttack_()) return 1;

        if (!cardSource.IsEnhanced()) return 1;

        return 1 + cardSource.GetEnhanceMultiplier();
    }

    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource == null) return 1;

        if (!props.IsPoweredCardOrMonsterMoveBlock_()) return 1;

        if (!cardSource.IsEnhanced()) return 1;

        return 1 + cardSource.GetEnhanceMultiplier();
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay is { Card: Runesmith2Card runesmithCard, IsLastInSeries: true })
        {
            runesmithCard.IsPlayedWithoutElements = false;
        } 
        
        if (cardPlay.Card.IsStasis()) return Task.CompletedTask;
        if (cardPlay.IsLastInSeries && cardPlay.Card.IsEnhanced()) cardPlay.Card.ClearEnhance();

        return Task.CompletedTask;
    }


    public decimal ModifyPotencyMultiplicative(Player player, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource == null) return 1;

        if (!props.IsPoweredCardOrMonsterMoveBlock_()) return 1;

        if (!cardSource.IsEnhanced()) return 1;

        return 1 + cardSource.GetEnhanceMultiplier();
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        return base.AfterPlayerTurnStart(choiceContext, player);
    }

    public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        return base.BeforeTurnEnd(choiceContext, side);
    }
}