using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Entities.Runes;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Field;

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor)]
[HarmonyPatch([typeof(Player)])]
internal class PlayerCombatStateConstructorPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player player, PlayerCombatState __instance)
    {
        var runeQueue = new RuneQueue(player);
        runeQueue.Clear();

        var runesmithCombatState = new PlayerCombatStateExtension.RunesmithCombatState(__instance, runeQueue);

        RunesmithField.RunesmithCombatState[__instance] = runesmithCombatState;

        CombatManager.Instance.StateTracker.SubscribeElements(runesmithCombatState);
    }
}

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.AfterCombatEnd))]
internal class PlayerCombatStateAfterCombatEndPatch
{
    [HarmonyPostfix]
    public static void Postfix(PlayerCombatState __instance)
    {
        var runesmithCombatState = __instance.Runesmith();
        if (runesmithCombatState != null) CombatManager.Instance.StateTracker.UnsubscribeElements(runesmithCombatState);
    }
}

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
internal class PlayerCombatStateHasEnoughResourcesForPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldarg_2()
            .opcode(OpCodes.Ldc_I4_0)
            .opcode(OpCodes.Stind_I4)
        ).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadArgument(1),
            CodeInstruction.LoadArgument(2),
            CodeInstruction.Call(typeof(PlayerCombatStateHasEnoughResourcesForPatch), nameof(HasEnoughElements))
        ]);
    }

    private static void HasEnoughElements(PlayerCombatState instance, CardModel card, ref UnplayableReason reason)
    {
        if (card is not Runesmith2Card runesmith2Card) return;
        if (!instance.Elements().CanSpend(runesmith2Card.GetElementsCostWithModifiers()))
            reason |= UnplayableReason.StarCostTooHigh;
    }
}