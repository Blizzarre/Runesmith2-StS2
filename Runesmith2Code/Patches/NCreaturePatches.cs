#region

using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Nodes.Runes;

#endregion

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(NCreature), nameof(NCreature._Ready))]
internal class NCreatureReadyPatch
{
    private static void UpdateRuneNavigation(NCreature __instance)
    {
        var runeManager = RunesmithNode.NRuneManager[__instance];
        if (runeManager != null) __instance.Hitbox.FocusNeighborTop = runeManager.DefaultFocusOwner.GetPath();
    }

    [HarmonyPrefix]
    private static void Prefix(NCreature __instance)
    {
        if (!__instance.Entity.IsPlayer) return;
        var runeManager = NRuneManager.Create(__instance, LocalContext.IsMe(__instance.Entity));
        __instance.AddChildSafely(runeManager);
        runeManager.Position = Vector2.Zero;
        RunesmithNode.NRuneManager[__instance] = runeManager;
    }
}

// TODO Fix async method patch
//  Rework this to transpiler patch?
[HarmonyPatch(typeof(NCreature), "AnimDie")]
internal class NCreatureAnimDiePatch
{
    [HarmonyPostfix]
    private static async Task Postfix(Task results, NCreature __instance)
    {
        await results;
        if (!RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
        {
            var runeManager = RunesmithNode.NRuneManager[__instance];
            runeManager?.ClearRunes();
        }
    }
}

[HarmonyPatch(typeof(NCreature), "OnCombatEnded")]
internal class NCreatureOnCombatEndedPatch
{
    [HarmonyPrefix]
    private static void Prefix(NCreature __instance)
    {
        var runeManager = RunesmithNode.NRuneManager[__instance];
        runeManager?.ClearRunes();
    }
}