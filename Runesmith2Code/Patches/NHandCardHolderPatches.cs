using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using Runesmith2.Runesmith2Code.Extensions;

namespace Runesmith2.Runesmith2Code.Patches;

// Patch to trigger card flash on card being enhanced or stasis
[HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.SubscribeToEvents))]
class NHandCardHolderSubscribePatch
{
    [HarmonyPrefix]
    static void SubscribeToEventsPrefix(
        NHandCardHolder __instance, ref CardModel? card
    )
    {
        if (__instance.CardNode != null)
        {
            if (card == null) return;
            var modifier = card.GetCardModelModifier();
            modifier.EnhanceChanged += __instance.Flash;
            modifier.StasisChanged += __instance.Flash;
        }
    }
}

[HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.UnsubscribeFromEvents))]
class NHandCardHolderUnsubscribePatch
{
    [HarmonyPrefix]
    static void UnsubscribeFromEventsPrefix(
        NHandCardHolder __instance, ref CardModel? card
    )
    {
        if (card == null) return;
        var modifier = card.GetCardModelModifier();
        modifier.EnhanceChanged -= __instance.Flash;
        modifier.StasisChanged -= __instance.Flash;
    }
}

[HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.Flash))]
class NHandCardHolderFlashPatch
{
    private static readonly Color BeigeGlow = new("dfbd81fa");

    private static void ShouldGlowBeige(NHandCardHolder instance)
    {
        var cardModel = instance.CardNode?.Model;
        if (cardModel == null)
        {
            return;
        }

        if (!CombatManager.Instance.IsPlayPhase) return;
        var modifier = cardModel.GetCardModelModifier();
        if (modifier.JustEnhanced)
        {
            instance._flash.Modulate = BeigeGlow;
        }
    }

    [HarmonyTranspiler]
    static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldarg_0()
            .call(typeof(NHandCardHolder), "get_ShouldGlowGold")
            .brfalse_s()
        ).Step(-3).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.Call((NHandCardHolder instance) => ShouldGlowBeige(instance))
        ]);
    }
}