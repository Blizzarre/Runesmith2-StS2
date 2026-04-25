using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Field;

namespace Runesmith2.Runesmith2Code.Patches;

// TODO check double calls for async method

// Ties the RuneQueue start and end turn triggers to the OrbQueue's triggers
[HarmonyPatch(typeof(OrbQueue), nameof(OrbQueue.AfterTurnStart))]
internal class OrbQueueAfterTurnStartPatch
{
    [HarmonyPostfix]
    private static async Task Postfix(Task results, PlayerChoiceContext choiceContext, OrbQueue __instance)
    {
        await results;
        var playerCombatState = __instance._owner.PlayerCombatState;
        if (playerCombatState == null) return;
        var runeQueue = RunesmithField.RunesmithCombatState[playerCombatState]?.RuneQueue;
        if (runeQueue == null) return;

        await runeQueue.AfterTurnStart(choiceContext);
    }
}

[HarmonyPatch(typeof(OrbQueue), nameof(OrbQueue.BeforeTurnEnd))]
internal class OrbQueueBeforeTurnEndPatch
{
    [HarmonyPostfix]
    private static async Task Postfix(Task results, PlayerChoiceContext choiceContext, OrbQueue __instance)
    {
        await results;
        var playerCombatState = __instance._owner.PlayerCombatState;
        if (playerCombatState == null) return;
        var runeQueue = RunesmithField.RunesmithCombatState[playerCombatState]?.RuneQueue;
        if (runeQueue == null) return;

        await runeQueue.BeforeTurnEnd(choiceContext);
    }
}