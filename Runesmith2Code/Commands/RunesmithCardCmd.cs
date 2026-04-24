using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;

namespace Runesmith2.Runesmith2Code.Commands;

public static class RunesmithCardCmd
{
    public static async Task Enhance(PlayerChoiceContext choiceContext, Player player, CardModel targetCard,
        CardPlay? cardPlay, int enhanceAmount, bool skipVisuals = false)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            if (!targetCard.IsEnhanceable())
            {
                throw new InvalidOperationException($"Cannot enhance {targetCard.Id}.");
            }

            var combatState = targetCard.CombatState ?? targetCard.Owner.Creature.CombatState;
            // TODO Consider adding history for cards enhanced.
            var modifiedEnhance =
                RunesmithHook.ModifyEnhanceAmount(combatState, player, enhanceAmount, out var modifiers);
            await RunesmithHook.AfterModifyingEnhanceAmount(combatState, modifiedEnhance, cardPlay?.Card, cardPlay,
                modifiers);
            targetCard.AddEnhance(enhanceAmount);
            // TODO Enhance vfx
            await RunesmithHook.AfterCardEnhanced(combatState, choiceContext, targetCard, enhanceAmount);
        }
    }

    public static async Task EnhanceRandomCards(PlayerChoiceContext choiceContext, Player player,
        IEnumerable<CardModel> cards, int cardCount, int enhanceBy, Rng rng, bool skipVisuals = false)
    {
        var randomCards = new List<CardModel>(cards).StableShuffle(rng);
        cardCount = Math.Min(cardCount, randomCards.Count);
        for (var i = 0; i < cardCount; i++)
        {
            await Enhance(choiceContext, player, randomCards[i], null, enhanceBy);
        }
    }
}