using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Runesmith2.Runesmith2Code.HoverTips;

public static class RunesmithHoverTipFactory
{
    public static IHoverTip Static(RunesmithHoverTip tip, params DynamicVar[] vars)
    {
        string text = tip.GetType().GetPrefix() + StringHelper.Slugify(tip.ToString());
        LocString locString = L10NStatic(text + ".title");
        LocString locString2 = L10NStatic(text + ".description");
        foreach (DynamicVar dynamicVar in vars)
        {
            locString.Add(dynamicVar);
            locString2.Add(dynamicVar);
        }
        return new HoverTip(locString, locString2);
    }
    
    public static LocString StaticBanner(RunesmithHoverTip tip, params DynamicVar[] vars)
    {
        string text = tip.GetType().GetPrefix() + StringHelper.Slugify(tip.ToString());
        LocString locString = L10NStatic(text + ".banner");
        foreach (DynamicVar dynamicVar in vars)
        {
            locString.Add(dynamicVar);
        }
        return locString;
    }
    
    private static LocString L10NStatic(string entry)
    {
        return new LocString("static_hover_tips", entry);
    }
}