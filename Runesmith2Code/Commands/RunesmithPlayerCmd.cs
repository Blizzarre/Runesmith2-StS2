using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Commands;

public static class RunesmithPlayerCmd
{
    public static async Task GainElements(Elements amount, Player player)
    {
        if (amount.Total > 0 && !CombatManager.Instance.IsEnding)
        {
            var combatState = player.Creature.CombatState;
            var runesmithCombatState = RunesmithField.RunesmithCombatState[player.PlayerCombatState];
            var finalAmount = RunesmithHook.ModifyElementsGain(combatState, player, amount, out var modifiers);
            await RunesmithHook.AfterModifyingElementsGain(combatState, modifiers);
            if (finalAmount.Total > 0)
            {
                // TODO play sfx
                runesmithCombatState.GainElements(finalAmount);
            }
        }
    }

    public static Task LoseElements(Elements amount, Player player)
    {
        if (amount.Total <= 0 || CombatManager.Instance.IsEnding)
        {
            return Task.CompletedTask;
        }

        var runesmithCombatState = RunesmithField.RunesmithCombatState[player.PlayerCombatState];
        runesmithCombatState.LoseElements(amount);
        return Task.CompletedTask;
    }

    public static async Task ResetElements(Player player)
    {
        if (!CombatManager.Instance.IsEnding)
        {
            var runesmithCombatState = RunesmithField.RunesmithCombatState[player.PlayerCombatState];
            var elements = runesmithCombatState.Elements;
            await LoseElements(elements, player);
        }
    }
}