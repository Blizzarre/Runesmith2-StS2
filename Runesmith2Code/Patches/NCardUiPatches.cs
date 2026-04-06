using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Nodes;

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(NCard), nameof(NCard._EnterTree))]
class NCardEnhanceTabPatch
{
    private static readonly PackedScene EnhanceTabScene = GD.Load<PackedScene>("res://Runesmith2/scenes/cards/enhance_tab.tscn");
    
    [HarmonyPrefix]
    static void RunesmithCardEnhanceUiPrefix(
        NCard __instance
    )
    {
        if (RunesmithField.NCardEnhanceTab[__instance] != null) return;
        var enhanceTab = EnhanceTabScene.Instantiate<NEnhanceTab>().WithData(__instance);
        RunesmithField.NCardEnhanceTab[__instance] = enhanceTab;
        var cardContainer = __instance.GetChild(0);
        if (cardContainer == null) return;
        cardContainer.AddChildSafely(enhanceTab);
        enhanceTab.Size = new Vector2(162, 40);
        enhanceTab.Position = new Vector2(-81, -171);
        cardContainer.MoveChild(enhanceTab, cardContainer.GetNode("TitleBanner").GetIndex());
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.SubscribeToModel))]
class NCardSubscribePatch
{
    [HarmonyPrefix]
    static void SubscribeToModelPrefix(
        NCard __instance, CardModel? model
    )
    {
        var enhanceTab = RunesmithField.NCardEnhanceTab[__instance];
        if (model == null || enhanceTab == null) return;
        var modifier = model.GetCardModelModifier();
        modifier.EnhanceChanged += enhanceTab.OnEnhanceChanged;
        modifier.StasisChanged += enhanceTab.OnStasisChanged;
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.UnsubscribeFromModel))]
class NCardUnsubscribePatch
{
    [HarmonyPrefix]
    static void UnsubscribeFromModelPrefix(
        NCard __instance, CardModel? model
    )
    {
        var enhanceTab = RunesmithField.NCardEnhanceTab[__instance];
        if (model == null || enhanceTab == null) return;
        var modifier = model.GetCardModelModifier();
        modifier.EnhanceChanged -= enhanceTab.OnEnhanceChanged;
        modifier.StasisChanged -= enhanceTab.OnStasisChanged;
    }
}

// TODO Implement check for forceUnpoweredPreview

[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
class NCardUpdateVisualsPatch
{
    [HarmonyPrefix]
    static void UpdateVisualsPrefix(NCard __instance)
    {
        if (!__instance.IsNodeReady()) return;
        if (__instance.Model == null) return;
        var enhanceTab = RunesmithField.NCardEnhanceTab[__instance];
        enhanceTab?.UpdateEnhanceVisuals();
    }
}