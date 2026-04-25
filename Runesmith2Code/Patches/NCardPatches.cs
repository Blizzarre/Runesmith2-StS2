using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Nodes;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Patches;

// Patch to add enhance and stasis visuals to card display
// Need to instantiate NEnhanceTab before it get accessed during Ready call because of SubscribeToModel
[HarmonyPatch(typeof(NCard), nameof(NCard._EnterTree))]
internal class NCardEnterTreePatch
{
    [HarmonyPrefix]
    private static void Prefix(NCard __instance)
    {
        if (RunesmithNode.NEnhanceTab[__instance] != null) return;
        var enhanceTab = PreloadManager.Cache.GetScene(RunesmithResource.NEnhanceTabPath)
            .Instantiate<NEnhanceTabContainer>()
            .WithData(__instance);
        RunesmithNode.NEnhanceTab[__instance] = enhanceTab;
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard._Ready))]
internal class NCardReadyPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        var enhanceTab = RunesmithNode.NEnhanceTab[__instance];
        if (enhanceTab == null) return;
        var cardContainer = __instance.GetChild(0);
        if (cardContainer == null) return;
        cardContainer.AddChildSafely(enhanceTab);
        cardContainer.MoveChild(enhanceTab, cardContainer.GetNode("%TitleBanner").GetIndex());
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.SubscribeToModel))]
internal class NCardSubscribePatch
{
    [HarmonyPrefix]
    private static void Prefix(
        NCard __instance, CardModel? model
    )
    {
        var enhanceTab = RunesmithNode.NEnhanceTab[__instance];
        if (model == null || enhanceTab == null) return;
        var modifier = model.GetCardModelModifier();
        modifier.EnhanceChanged += enhanceTab.OnEnhanceChanged;
        modifier.StasisChanged += enhanceTab.OnStasisChanged;
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.UnsubscribeFromModel))]
internal class NCardUnsubscribePatch
{
    [HarmonyPrefix]
    private static void Prefix(
        NCard __instance, CardModel? model
    )
    {
        var enhanceTab = RunesmithNode.NEnhanceTab[__instance];
        if (model == null || enhanceTab == null) return;
        var modifier = model.GetCardModelModifier();
        modifier.ClearFlags();
        modifier.EnhanceChanged -= enhanceTab.OnEnhanceChanged;
        modifier.StasisChanged -= enhanceTab.OnStasisChanged;
    }
}

// TODO Implement check for forceUnpoweredPreview?
[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
internal class NCardUpdateVisualsPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldarg_0()
            .ldarg_1()
            .call(typeof(NCard), nameof(NCard.UpdateStarCostVisuals), [typeof(PileType)])
        ).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadArgument(1),
            CodeInstruction.Call(typeof(NCardUpdateVisualsPatch), nameof(UpdateRunesmithVisuals))
        ]);
    }

    private static void UpdateRunesmithVisuals(NCard instance, PileType pileType)
    {
        var enhanceTab = RunesmithNode.NEnhanceTab[instance];
        enhanceTab?.UpdateEnhanceVisuals();

        var elementsIcon = RunesmithNode.NElementsIcon[instance];
        elementsIcon?.UpdateElementsCostVisuals(pileType);
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.SetPretendCardCanBePlayed))]
internal class NCardSetPretendCardCanBePlayedPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        var elementsIcon = RunesmithNode.NElementsIcon[__instance];
        elementsIcon?.UpdateElementsCostVisuals(__instance.DisplayingPile);
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateEnchantmentVisuals))]
internal class NCardUpdateEnchantmentVisualsPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        if (__instance.Model is Runesmith2Card { BaseElementsCost.Total: >= 0 })
            __instance._enchantmentTab.Position = __instance._defaultEnchantmentPosition + Vector2.Down * 20f;
    }
}