#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Runesmith2.Runesmith2Code.Nodes;
using Runesmith2.Runesmith2Code.Nodes.Runes;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Field;

public static class RunesmithNode
{
    public static readonly AddedNode<NCombatUi, NElementsCounter> NElementsCounter = new(ui =>
    {
        var elementsCounter = PreloadManager.Cache.GetScene(RunesmithResource.NElementsCounterPath)
            .Instantiate<NElementsCounter>();
        ui.AddChildSafely(elementsCounter);
        return elementsCounter;
    });

    public static readonly AddedNode<NCard, NElementsIcon> NElementsIcon = new(card =>
    {
        var elementsIcon = PreloadManager.Cache.GetScene(RunesmithResource.NElementsIconPath)
            .Instantiate<NElementsIcon>().WithData(card);
        var cardContainer = card.GetChild(0)!;
        cardContainer.AddChildSafely(elementsIcon);
        cardContainer.MoveChild(elementsIcon, cardContainer.GetNode("%StarIcon").GetIndex());
        return elementsIcon;
    });

    public static readonly SpireField<NCard, NEnhanceTabContainer> NEnhanceTab = new(() => null);

    public static readonly SpireField<NCreature, NRuneManager> NRuneManager = new(() => null);
}