using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Nodes.Runes;

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(NCreature), nameof(NCreature._Ready))]
class NCreatureReadyPatch
{
    static void UpdateRuneNavigation(NCreature __instance)
    {
        var runeManager = RunesmithNode.NRuneManager[__instance];
        if (runeManager != null)
        {
            __instance.Hitbox.FocusNeighborTop = runeManager.DefaultFocusOwner.GetPath();
        }
    }

    [HarmonyPrefix]
    static void Prefix(NCreature __instance)
    {
        if (!__instance.Entity.IsPlayer) return;
        var runeManager = NRuneManager.Create(__instance, LocalContext.IsMe(__instance.Entity));
        __instance.AddChildSafely(runeManager);
        runeManager.Position = Vector2.Zero;
        RunesmithNode.NRuneManager[__instance] = runeManager;
    }
}

[HarmonyPatch(typeof(NCreature), "AnimDie")]
class NCreatureAnimDiePatch
{
    [HarmonyPostfix]
    static async Task Postfix(Task results, NCreature __instance)
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
class NCreatureOnCombatEndedPatch
{
    [HarmonyPrefix]
    static void Prefix(NCreature __instance)
    {
        var runeManager = RunesmithNode.NRuneManager[__instance];
        runeManager?.ClearRunes();
    }
}