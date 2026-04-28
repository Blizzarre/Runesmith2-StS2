using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Field;

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(NCardPlay), nameof(NCardPlay.TryShowEvokingOrbs))]
static class NCardPlayTryShowEvokingOrbsPatch
{
    [HarmonyPostfix]
    static void Postfix(NCardPlay __instance)
    {
        var owner = __instance.CardOwnerNode;
        var card = __instance.Card;
        if (card == null || owner == null) return;
        if (card is not Runesmith2Card runesmithCard) return;
        var runeManager = RunesmithNode.NRuneManager[owner];
        runeManager?.UpdateVisuals(runesmithCard.RuneBreakType);
    }
}

[HarmonyPatch(typeof(NCardPlay), nameof(NCardPlay.HideEvokingOrbs))]
static class NCardPlayHideEvokingOrbsPatch
{
    [HarmonyPostfix]
    static void Postfix(NCardPlay __instance)
    {
        var owner = __instance.CardOwnerNode;
        if (owner == null) return;
        var runeManager = RunesmithNode.NRuneManager[owner];
        runeManager?.UpdateVisuals(RuneBreakType.None);
    }
}