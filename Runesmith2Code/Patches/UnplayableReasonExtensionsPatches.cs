using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(UnplayableReasonExtensions), nameof(UnplayableReasonExtensions.GetPlayerDialogueLine))]
public class UnplayableReasonExtensionsGetPlayerDialogueLinePatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref LocString? __result, UnplayableReason reason, AbstractModel? preventer)
    {
        MainFile.Logger.Info($"GetPlayerDialogueLinePatch {__result} {reason} {preventer}");
        if (reason.HasFlag(UnplayableReason.StarCostTooHigh) && preventer is Runesmith2Card)
        {
            __result = new LocString("combat_messages", "RUNESMITH2-NOT_ENOUGH_ELEMENTS");
            return false;
        }

        return true;
    }
}