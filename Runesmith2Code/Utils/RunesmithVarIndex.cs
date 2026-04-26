#region

using MegaCrit.Sts2.Core.Localization.DynamicVars;

#endregion

namespace Runesmith2.Runesmith2Code.Utils;

public static class RunesmithVarIndex
{
    public static readonly StringVar IgnisIconVar =
        new("IgnisIcon", "[img]res://Runesmith2/images/charui/elements_ignis_icon.png[/img]");

    public static readonly StringVar TerraIconVar =
        new("TerraIcon", "[img]res://Runesmith2/images/charui/elements_terra_icon.png[/img]");

    public static readonly StringVar AquaIconVar =
        new("AquaIcon", "[img]res://Runesmith2/images/charui/elements_aqua_icon.png[/img]");

    public static readonly StringVar ElementsIconVar =
        new("ElementsIcon", "[img]res://Runesmith2/images/charui/elements_all_icon.png[/img]");
}