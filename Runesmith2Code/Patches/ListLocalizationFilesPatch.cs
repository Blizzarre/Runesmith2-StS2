using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace Runesmith2.Runesmith2Code.Patches;

// Code mostly taken from https://github.com/lamali292/Downfall/blob/main/Code/Patches/ListLocalizationFilesPatch.cs
[HarmonyPatch(typeof(LocManager), "ListLocalizationFiles")]
static class ListLocalizationFilesPatch
{
    private static readonly string[] ExtraTables = ["runes.json"];

    [HarmonyPostfix]
    static void Postfix(ref IEnumerable<string> __result)
    {
        var result = __result;
        var enumerable = result as string[] ?? result.ToArray();
        __result = enumerable.Concat(ExtraTables.Where(f => !enumerable.Contains(f)));
    }
}

[HarmonyPatch(typeof(LocManager), "LoadTable")]
static class LoadTablePatch
{
    private static readonly string[] ExtraTables = ["runes.json"];

    static bool Prefix(string path, ref Dictionary<string, string> __result)
    {
        if (Godot.FileAccess.FileExists(path)) return true;
        if (!ExtraTables.Any(path.Contains)) return true;
        // Only override result if file does not exist and path is known to be in ExtraTables.
        __result = new Dictionary<string, string>();
        return false;
    }
}