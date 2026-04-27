using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Powers;

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(PowerModel), nameof(PowerModel.AddDumbVariablesToDescription))]
class PowerModelAddDumbVariablesToDescriptionPatche
{
    [HarmonyPostfix]
    static void Postfix(PowerModel __instance, LocString description)
    {
        if (__instance is Runesmith2Power)
        {
            description.Add("elements", 0);
        }
    }
}