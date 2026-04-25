using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using Runesmith2.Runesmith2Code.Nodes.Runes;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Patches;

// Add custom resources to preload list
[HarmonyPatch(typeof(AssetSets), nameof(AssetSets.CommonAssets), MethodType.Getter)]
internal static class AssetSetsCommonAssetPatch
{
    [HarmonyPostfix]
    private static void Postfix(ref IReadOnlySet<string> __result)
    {
        __result = __result.Concat(RunesmithResource.AssetPaths).ToHashSet();
    }
}