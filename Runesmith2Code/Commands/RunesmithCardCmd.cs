using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;

namespace Runesmith2.Runesmith2Code.Commands;

public static class RunesmithCardCmd
{
    public static async Task Enhance(PlayerChoiceContext choiceContext, CardModel card, int enhanceAmount, bool skipVisuals = false)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            if (!card.IsEnhanceable())
            {
                throw new InvalidOperationException($"Cannot enhance {card.Id}.");
            }
            var combatState = card.CombatState ?? card.Owner.Creature.CombatState;
            // TODO Consider adding history for cards enhanced.
            
            // TODO modify enhanceAmount based on active powers... maybe use hooks to modify the value?
            card.AddEnhance(enhanceAmount);
            // TODO Enhance vfx
            // trigger the flash in nhandcardholder via subscriber
            await RunesmithHook.AfterCardEnhanced(combatState, choiceContext, card, enhanceAmount);
        }
    }
}