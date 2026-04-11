using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Field;

namespace Runesmith2.Runesmith2Code.Patches;

// Ties the RuneQueue start and end turn triggers to the OrbQueue's triggers
// TODO this might fail (async patching)
[HarmonyPatch(typeof(OrbQueue), nameof(OrbQueue.AfterTurnStart))]
class OrbQueueAfterTurnStartPatch
{
    [HarmonyPostfix]
    static async Task Postfix(Task results, PlayerChoiceContext choiceContext, OrbQueue __instance)
    {
        await results;
        // TODO this might not be fool proof? this assumes that the Player.PlayerCombatState.OrbQueue is the same as OrbQueue._owner
        var playerCombatState = __instance._owner.PlayerCombatState;
        if (playerCombatState == null) return;
        var runeQueue = RunesmithField.RuneQueue[playerCombatState];
        if (runeQueue == null) return;

        await runeQueue.AfterTurnStart(choiceContext);
    }
}

[HarmonyPatch(typeof(OrbQueue), nameof(OrbQueue.BeforeTurnEnd))]
class OrbQueueBeforeTurnEndPatch
{
    [HarmonyPostfix]
    static async Task Postfix(Task results, PlayerChoiceContext choiceContext, OrbQueue __instance)
    {
        await results;
        // TODO this might not be fool proof? this assumes that the Player.PlayerCombatState.OrbQueue is the same as OrbQueue._owner
        var playerCombatState = __instance._owner.PlayerCombatState;
        if (playerCombatState == null) return;
        var runeQueue = RunesmithField.RuneQueue[playerCombatState];
        if (runeQueue == null) return;

        await runeQueue.BeforeTurnEnd(choiceContext);
    }
}