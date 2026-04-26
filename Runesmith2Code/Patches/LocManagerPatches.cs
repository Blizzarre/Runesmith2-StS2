#region

using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using Runesmith2.Runesmith2Code.Formatters;
using SmartFormat;

#endregion

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(LocManager), nameof(LocManager.LoadLocFormatters))]
public class LocManagerLoadLocFormattersPatch
{
    [HarmonyPostfix]
    private static void AddCustomFormatters()
    {
        Smart.Default.AddExtensions(new ElementsIconsFormatter());
    }
}