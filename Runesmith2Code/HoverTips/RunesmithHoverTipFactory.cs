#region

using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Models;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.HoverTips;

public static class RunesmithHoverTipFactory
{
    
    public static IHoverTip CreateElementsHoverTip()
    {
        const RunesmithHoverTip tip = RunesmithHoverTip.Elements;
        var text = tip.GetType().GetPrefix() + StringHelper.Slugify(tip.ToString());

        return Static(text, RunesmithResource.ElementsIcon);
    }
    
    public static IHoverTip Static(RunesmithHoverTip tip, params DynamicVar[] vars)
    {
        var text = tip.GetType().GetPrefix() + StringHelper.Slugify(tip.ToString());
        return Static(text, null, vars);
    }

    public static IHoverTip Static(string entry, params DynamicVar[] vars)
    {
        return Static(entry, null, vars);
    }

    public static IHoverTip Static(string entry, Texture2D? icon, params DynamicVar[] vars)
    {
        var locString = L10NStatic(entry + ".title");
        var locString2 = L10NStatic(entry + ".description");
        foreach (var dynamicVar in vars)
        {
            locString.Add(dynamicVar);
            locString2.Add(dynamicVar);
        }
        locString2.Add("elements", 0);

        return new HoverTip(locString, locString2, icon);
    }

    public static LocString StaticBanner(RunesmithHoverTip tip, params DynamicVar[] vars)
    {
        var text = tip.GetType().GetPrefix() + StringHelper.Slugify(tip.ToString());
        var locString = L10NStatic(text + ".banner");
        foreach (var dynamicVar in vars) locString.Add(dynamicVar);

        return locString;
    }

    private static LocString L10NStatic(string entry)
    {
        return new LocString("static_hover_tips", entry);
    }

    public static IHoverTip FromRune<T>() where T : RuneModel
    {
        return ModelDb.Get<T>().DumbHoverTip;
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