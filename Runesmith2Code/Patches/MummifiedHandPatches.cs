using System.Reflection;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Runesmith2.Runesmith2Code.Cards;

namespace Runesmith2.Runesmith2Code.Patches;

// Patch to include cards with Elements cost in MummifiedHand first check
[HarmonyPatch(typeof(MummifiedHand), nameof(MummifiedHand.AfterCardPlayed))]
public class MummifiedHandPatches
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions)
            .Match(new InstructionMatcher()
                .dup()
                .any()
                .call_any().PredicateMatch(op => op is MethodInfo { Name: "Where" } methodInfo && methodInfo.DeclaringType == typeof(Enumerable))
                .call_any().PredicateMatch(op => op is MethodInfo { Name: "ToList" } methodInfo && methodInfo.DeclaringType == typeof(Enumerable))
                .stloc_2()
            ).Insert([
                CodeInstruction.LoadLocal(1), //Load cards
                CodeInstruction.LoadLocal(2), //Load list
                CodeInstruction.Call(typeof(MummifiedHandPatches), nameof(FilterForElementsCards)),
                CodeInstruction.StoreLocal(2) //Store list
            ]);
    }

    private static List<CardModel> FilterForElementsCards(IReadOnlyList<CardModel> cards, List<CardModel> list)
    {
        var set = list.ToHashSet();
        return cards.Where(c => set.Contains(c) || c is Runesmith2Card { BaseElementsCost.Total: > 0 }).ToList();
    }
}