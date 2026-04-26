#region

using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Runesmith2.Runesmith2Code.Field;

#endregion

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal class NCombatUiPatches
{
    [HarmonyPostfix]
    private static void Postfix(NCombatUi __instance, CombatState state)
    {
        var elementsCounter = RunesmithNode.NElementsCounter[__instance];
        if (elementsCounter == null) return;
        elementsCounter.Initialize(LocalContext.GetMe(state)!);
        elementsCounter.Reparent(__instance._energyCounter);
        elementsCounter.Position = new Vector2(0, -120);
        elementsCounter.Size = new Vector2(128, 128);
    }
}