#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;

#endregion

namespace Runesmith2.Runesmith2Code.Commands;

public static class RunesmithCardCmd
{
    public static async Task Enhance(PlayerChoiceContext choiceContext, Player player,
        CardModel targetCard,
        CardPlay? cardPlay, int enhanceAmount, bool skipVisuals = false)
    {
        await Enhance(choiceContext, player, [targetCard], cardPlay, enhanceAmount, skipVisuals);
    }
    
    public static async Task Enhance(PlayerChoiceContext choiceContext, Player player, IEnumerable<CardModel> targetCards,
        CardPlay? cardPlay, int enhanceAmount, bool skipVisuals = false)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            var combatState = player.Creature.CombatState;
            var cardList = targetCards.ToList();
            if (combatState == null || cardList.Count == 0) return;
            // TODO Consider adding history for cards enhanced.
            var modifiedEnhance =
                RunesmithHook.ModifyEnhanceAmount(combatState, player, enhanceAmount, cardPlay?.Card,
                    out var modifiers);
            await RunesmithHook.AfterModifyingEnhanceAmount(combatState, modifiedEnhance, cardPlay?.Card, cardPlay,
                modifiers);

            foreach (var targetCard in cardList)
            {
                if (!targetCard.CanEnhance()) throw new InvalidOperationException($"Cannot enhance {targetCard.Id}.");
            
                targetCard.AddEnhance(enhanceAmount);
                // TODO Enhance vfx
                await RunesmithHook.AfterCardEnhanced(combatState, choiceContext, targetCard, enhanceAmount);
            }
        }
    }

    public static async Task EnhanceRandomCards(PlayerChoiceContext choiceContext, Player player,
        IEnumerable<CardModel> cards, int cardCount, int enhanceBy, Rng rng, bool skipVisuals = false)
    {
        var randomCards = new List<CardModel>(cards.Where(c => c.CanEnhance())).StableShuffle(rng);
        await Enhance(choiceContext, player, randomCards.Take(cardCount), null, enhanceBy);
    }

    public static bool Stasis(CardModel targetCard)
    {
        if (CombatManager.Instance.IsOverOrEnding) return false;
        if (!targetCard.CanEnhance()) throw new InvalidOperationException($"Cannot enhance {targetCard.Id}.");

        if (targetCard.IsStasis()) return false;

        targetCard.SetStasis(true);
        return true;
    }
}