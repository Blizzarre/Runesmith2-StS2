using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Extensions;
using static Runesmith2.Runesmith2Code.Extensions.PlayerCombatStateExtension;

namespace Runesmith2.Runesmith2Code.Field;

public static class RunesmithField
{
    public static readonly SpireField<CardModel, CardModelExtension.RunesmithCardModelModifier> Modifier =
        new(() => null);

    public static readonly SpireField<PlayerCombatState, RunesmithCombatState> RunesmithCombatState = new(() => null);
}