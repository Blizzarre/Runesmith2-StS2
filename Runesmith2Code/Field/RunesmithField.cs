using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Runesmith2.Runesmith2Code.Entities.Runes;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Nodes;
using Runesmith2.Runesmith2Code.Nodes.Runes;

namespace Runesmith2.Runesmith2Code.Field;

public static class RunesmithField
{
    public static readonly SpireField<CardModel, CardModelExtension.RunesmithCardModelModifier> Modifier =
        new(() => null);
    
    public static readonly SpireField<NCard, NEnhanceTab> NEnhanceTab = new(() => null);
    
    public static readonly SpireField<PlayerCombatState, RuneQueue> RuneQueue = new(() => null);
    public static readonly SpireField<NCreature, NRuneManager> NRuneManager = new(() => null);
}