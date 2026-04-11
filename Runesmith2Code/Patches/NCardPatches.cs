using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Nodes;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Patches;

// Patch to add enhance and stasis visuals to card display
// Need to instantiate NEnhanceTab before it get accessed during Ready call
[HarmonyPatch(typeof(NCard), nameof(NCard._EnterTree))]
class NCardEnterTreePatch
{
    [HarmonyPrefix]
    static void Prefix(
        NCard __instance
    )
    {
        if (RunesmithField.NEnhanceTab[__instance] != null) return;
        var enhanceTab = PreloadManager.Cache.GetScene(RunesmithResource.NEnhanceTabPath).Instantiate<NEnhanceTab>().WithData(__instance);
        RunesmithField.NEnhanceTab[__instance] = enhanceTab;
        var cardContainer = __instance.GetChild(0);
        if (cardContainer == null) return;
        cardContainer.AddChildSafely(enhanceTab);
        enhanceTab.SetAnchorsPreset(Control.LayoutPreset.Center, true);
        enhanceTab.Size = new Vector2(162, 40);
        enhanceTab.Position = new Vector2(-81, -171);
        cardContainer.MoveChild(enhanceTab, cardContainer.GetNode("TitleBanner").GetIndex());
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.SubscribeToModel))]
class NCardSubscribePatch
{
    [HarmonyPrefix]
    static void Prefix(
        NCard __instance, CardModel? model
    )
    {
        var enhanceTab = RunesmithField.NEnhanceTab[__instance];
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
    static void Prefix(
        NCard __instance, CardModel? model
    )
    {
        var enhanceTab = RunesmithField.NEnhanceTab[__instance];
        if (model == null || enhanceTab == null) return;
        var modifier = model.GetCardModelModifier();
        modifier.ClearFlags();
        modifier.EnhanceChanged -= enhanceTab.OnEnhanceChanged;
        modifier.StasisChanged -= enhanceTab.OnStasisChanged;
    }
}

// TODO Implement check for forceUnpoweredPreview?
[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
class NCardUpdateVisualsPatch
{
    [HarmonyPrefix]
    static void Prefix(NCard __instance)
    {
        if (!__instance.IsNodeReady()) return;
        if (__instance.Model == null) return;
        var enhanceTab = RunesmithField.NEnhanceTab[__instance];
        enhanceTab?.UpdateEnhanceVisuals();
    }
}