using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
class CardHoverTipPatch
{
    [HarmonyPostfix]
    static IEnumerable<IHoverTip> Postfix(
        IEnumerable<IHoverTip> values, CardModel __instance
    )
    {
        List<IHoverTip> list = values.ToList();
        if (__instance.IsEnhanced())
        {
            list.Add(RunesmithHoverTipFactory.Static(RunesmithHoverTip.Enhanced, new DynamicVar("Amount", __instance.GetEnhanceMultiplier() * 100)));
        }

        if (__instance.IsStasis())
        {
            list.Add(RunesmithHoverTipFactory.Static(RunesmithHoverTip.Stasis));
        }
        
        return list.Distinct();
    }
}