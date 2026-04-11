using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Models;

namespace Runesmith2.Runesmith2Code.HoverTips;

public static class RunesmithHoverTipFactory
{
    public static IHoverTip Static(RunesmithHoverTip tip, params DynamicVar[] vars)
    {
        var text = tip.GetType().GetPrefix() + StringHelper.Slugify(tip.ToString());
        var locString = L10NStatic(text + ".title");
        var locString2 = L10NStatic(text + ".description");
        foreach (var dynamicVar in vars)
        {
            locString.Add(dynamicVar);
            locString2.Add(dynamicVar);
        }
        return new HoverTip(locString, locString2);
    }
    
    public static LocString StaticBanner(RunesmithHoverTip tip, params DynamicVar[] vars)
    {
        var text = tip.GetType().GetPrefix() + StringHelper.Slugify(tip.ToString());
        var locString = L10NStatic(text + ".banner");
        foreach (var dynamicVar in vars)
        {
            locString.Add(dynamicVar);
        }
        return locString;
    }
    
    private static LocString L10NStatic(string entry)
    {
        return new LocString("static_hover_tips", entry);
    }

    public static HoverTip CreateRuneHoverTip(RuneModel rune, LocString description)
    {
        var hoverTip = new HoverTip
        {
            IsSmart = false,
            IsDebuff = false,
            IsInstanced = false,
            CanonicalModel = null,
            ShouldOverrideTextOverflow = false,
            Id = rune.Id.ToString(),
            Title = rune.Title.GetFormattedText(),
            Description = description.GetFormattedText(),
            Icon = rune.Icon
        };
        return hoverTip;
    }
}