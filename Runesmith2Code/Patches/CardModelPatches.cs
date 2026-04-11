using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Runesmith2.Runesmith2Code.Field;

namespace Runesmith2.Runesmith2Code.Patches;


[HarmonyPatch(typeof(CardModel), "AfterCloned")]
class CardModelAfterClonedPatch
{
    [HarmonyPostfix]
    static void Postfix(CardModel __instance)
    {
        RunesmithField.Modifier[__instance]?.ClearFlags();
    }
}