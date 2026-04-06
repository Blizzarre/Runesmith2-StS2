using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Nodes;

namespace Runesmith2.Runesmith2Code.Field;

public static class RunesmithField
{
    public static readonly SpireField<CardModel, CardModelExtension.RunesmithCardModelModifier> Modifier =
        new(() => null);

    // TODO will this work?
    public static readonly SpireField<NCard, NEnhanceTab> NCardEnhanceTab = new(() => null);
}