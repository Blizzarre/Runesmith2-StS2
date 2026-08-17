#region

using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Field;

#endregion

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(NCardPlay), "TryShowEvokingOrbs")]
internal static class NCardPlayTryShowEvokingOrbsPatch
{
    private static readonly PropertyInfo CardOwnerNodeProp = AccessTools.Property(typeof(NCardPlay), "CardOwnerNode");
    private static readonly PropertyInfo CardProp = AccessTools.Property(typeof(NCardPlay), "Card");

    [HarmonyPostfix]
    private static void Postfix(NCardPlay __instance)
    {
        var owner = (NCreature?)CardOwnerNodeProp.GetValue(__instance);
        var card = (CardModel?)CardProp.GetValue(__instance);
        if (card == null || owner == null) return;
        if (card is not Runesmith2Card runesmithCard) return;
        var runeManager = RunesmithNode.NRuneManager[owner];
        runeManager?.UpdateVisuals(runesmithCard.RuneBreakType);
    }
}

[HarmonyPatch(typeof(NCardPlay), nameof(NCardPlay.HideEvokingOrbs))]
internal static class NCardPlayHideEvokingOrbsPatch
{
    private static readonly PropertyInfo CardOwnerNodeProp = AccessTools.Property(typeof(NCardPlay), "CardOwnerNode");

    [HarmonyPostfix]
    private static void Postfix(NCardPlay __instance)
    {
        var owner = (NCreature?)CardOwnerNodeProp.GetValue(__instance);
        if (owner == null) return;
        var runeManager = RunesmithNode.NRuneManager[owner];
        runeManager?.UpdateVisuals(RuneBreakType.None);
    }
}