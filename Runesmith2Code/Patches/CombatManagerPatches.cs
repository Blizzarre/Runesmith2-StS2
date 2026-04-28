#region

using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using Runesmith2.Runesmith2Code.Commands;

#endregion

namespace Runesmith2.Runesmith2Code.Patches;

// TODO Fix async method patch
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.HandlePlayerDeath))]
public static class CombatManagerHandlePlayerDeathPatch
{
    [HarmonyPostfix]
    private static async Task Postfix(Task results, CombatManager __instance, Player player)
    {
        await results;
        if (__instance.IsInProgress) await RunesmithPlayerCmd.ResetElements(player);
    }
}