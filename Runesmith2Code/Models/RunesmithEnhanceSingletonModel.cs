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
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Models;

public class RunesmithEnhanceSingletonModel() : CustomSingletonModel(HookType.Combat), IModifyPotencyMultiplicative
{
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
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
            runesmithCard.IsPlayedWithoutElements = false;
        
        var card = cardPlay.Card;
        var isEnhanced = card.IsEnhanced();

        if (card.IsStasis() && isEnhanced)
        {
            card.SetEnhanceAfterClear(0);
            if (card is not Runesmith2Card { PreserveStasis: true })
            {
                card.SetStasis(false);
                RunesmithModSounds.PlayStasisUseSfx();
            }
            return Task.CompletedTask;
        }
        if (isEnhanced)
        {
            card.ClearEnhance();
        }

        var enhanceAfterClear = card.GetEnhanceAfterClear();
        if (enhanceAfterClear > 0)
        {
            card.SetEnhanceAfterClear(0);
            card.AddEnhance(enhanceAfterClear);
        }

        return Task.CompletedTask;
    }


    public decimal ModifyPotencyMultiplicative(Player player, decimal amount, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource == null) return 1;

        if (!props.IsPoweredCardOrMonsterMoveBlock_()) return 1;

        if (!cardSource.IsEnhanced()) return 1;

        return 1 + cardSource.GetEnhanceMultiplier();
    }

    // TODO: Temp solution before placing this inside CombatManager.SetupPlayerTurn
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        var runeQueue = player.PlayerCombatState != null
            ? RunesmithField.RunesmithCombatState[player.PlayerCombatState]?.RuneQueue
            : null;
        if (runeQueue != null) await runeQueue.SetupTurnStart(choiceContext);
    }
}