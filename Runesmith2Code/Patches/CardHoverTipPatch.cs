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
            // TODO Consider better way to calculate this (maybe there are other models that affect enhance effectiveness)
            list.Add(RunesmithHoverTipFactory.Static(RunesmithHoverTip.Enhanced, new DynamicVar("Amount", __instance.GetEnhance() * 50)));
        }

        if (__instance.IsStasis())
        {
            list.Add(RunesmithHoverTipFactory.Static(RunesmithHoverTip.Stasis));
        }
        
        return list.Distinct();
    }
}