#region

using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.DynamicVars;
using SmartFormat.Core.Extensions;
using static System.Int32;

#endregion

namespace Runesmith2.Runesmith2Code.Formatters;

public class ElementsIconsFormatter : IFormatter
{
    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        int amount;
        string iconText;
        switch (formattingInfo.CurrentValue)
        {
            case ElementsVar elementsVar:
                amount = Convert.ToInt32(elementsVar.PreviewValue);
                iconText = "[img]res://Runesmith2/images/charui/elements_all_icon.png[/img]";
                break;
            case IgnisVar ignisVar:
                amount = Convert.ToInt32(ignisVar.PreviewValue);
                iconText = "[img]res://Runesmith2/images/charui/elements_ignis_icon.png[/img]";
                break;
            case TerraVar terraVar:
                amount = Convert.ToInt32(terraVar.PreviewValue);
                iconText = "[img]res://Runesmith2/images/charui/elements_terra_icon.png[/img]";
                break;
            case AquaVar aquaVar:
                amount = Convert.ToInt32(aquaVar.PreviewValue);
                iconText = "[img]res://Runesmith2/images/charui/elements_aqua_icon.png[/img]";
                break;
            case DynamicVar dynVar:
                amount = Convert.ToInt32(dynVar.PreviewValue);
                iconText = GetElementsIcon(formattingInfo.FormatterOptions);
                break;
            case int value:
                amount = value;
                iconText = GetElementsIcon(formattingInfo.FormatterOptions);
                break;
            case decimal value:
                amount = (int)value;
                iconText = GetElementsIcon(formattingInfo.FormatterOptions);
                break;
            case null:
                amount = 0;
                iconText = GetElementsIcon(formattingInfo.FormatterOptions);
                break;
            default:
                throw new LocException(
                    $"Unknown value='{formattingInfo.CurrentValue}' type={formattingInfo.CurrentValue?.GetType()}");
        }

        if (formattingInfo.FormatterOptions.Contains('0')) amount = 0;

        var splitOpts = formattingInfo.FormatterOptions.Split(',');
        if (splitOpts.Length > 1)
            if (TryParse(splitOpts[1], out var newAmount))
                amount = newAmount;

        string finalText;
        if (amount <= 0)
            finalText = iconText;
        else if (formattingInfo.CurrentValue is DynamicVar dynamicVar)
            finalText = dynamicVar.ToHighlightedString(false) + " " + iconText;
        else
            finalText = $"{amount} {iconText}";

        formattingInfo.Write(finalText);

        return true;
    }

    private static string GetElementsIcon(string format)
    {
        var name = format.Split(",").First();
        return name switch
        {
            "ignis" => "[img]res://Runesmith2/images/charui/elements_ignis_icon.png[/img]",
            "terra" => "[img]res://Runesmith2/images/charui/elements_terra_icon.png[/img]",
            "aqua" => "[img]res://Runesmith2/images/charui/elements_aqua_icon.png[/img]",
            "all" or "element" or "elements" => "[img]res://Runesmith2/images/charui/elements_all_icon.png[/img]",
            _ => throw new LocException($"Unknown value='{format}'")
        };
    }

    // Prefixed formatter name to prevent conflict with other mods
    public string Name
    {
        get => "rs2Icon";
        set => throw new NotImplementedException();
    }

    public bool CanAutoDetect { get; set; }
}