using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Commands;

public static class RunesmithPlayerCmd
{
    public static async Task GainElements(Elements amount, Player player, CardPlay? cardPlay = null)
    {
        if (amount.Total > 0 && !CombatManager.Instance.IsEnding)
        {
            var combatState = player.Creature.CombatState;
            var runesmithCombatState = player.PlayerCombatState?.Runesmith();
            var finalAmount = RunesmithHook.ModifyElementsGain(combatState, player, amount, ValueProp.Move, cardPlay?.Card, out var modifiers);
            await RunesmithHook.AfterModifyingElementsGain(combatState, modifiers);
            if (finalAmount.Total > 0)
                // TODO play sfx
                runesmithCombatState?.GainElements(finalAmount);

            await RunesmithHook.AfterElementsGained(combatState, amount, player, cardPlay);
        }
    }

    public static Task LoseElements(Elements amount, Player player)
    {
        if (amount.Total <= 0 || CombatManager.Instance.IsEnding) return Task.CompletedTask;

        player.PlayerCombatState?.Runesmith()?.LoseElements(amount);
        return Task.CompletedTask;
    }

    public static async Task ResetElements(Player player)
    {
        if (!CombatManager.Instance.IsEnding)
        {
            var elements = player.PlayerCombatState?.Elements() ?? new Elements();
            await LoseElements(elements, player);
        }
    }
}