#region

using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using FileAccess = Godot.FileAccess;

#endregion

namespace Runesmith2.Runesmith2Code.Patches;

// Code mostly taken from https://github.com/lamali292/Downfall/blob/main/Code/Patches/ListLocalizationFilesPatch.cs
[HarmonyPatch(typeof(LocManager), "ListLocalizationFiles")]
internal static class ListLocalizationFilesPatches
{
    private static readonly string[] ExtraTables = ["runes.json"];

    [HarmonyPostfix]
    private static void Postfix(ref IEnumerable<string> __result)
    {
        var result = __result;
        var enumerable = result as string[] ?? result.ToArray();
        __result = enumerable.Concat(ExtraTables.Where(f => !enumerable.Contains(f)));
    }
}

[HarmonyPatch(typeof(LocManager), "LoadTable")]
internal static class LoadTablePatch
{
    private static readonly string[] ExtraTables = ["runes.json"];

    private static bool Prefix(string path, ref Dictionary<string, string> __result)
    {
        if (FileAccess.FileExists(path)) return true;
        if (!ExtraTables.Any(path.Contains)) return true;
        // Only override result if file does not exist and path is known to be in ExtraTables.
        __result = new Dictionary<string, string>();
        return false;
    }
}