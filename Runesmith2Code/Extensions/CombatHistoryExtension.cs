#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;
using Runesmith2.Runesmith2Code.Combat;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Extensions;

public static class CombatHistoryExtension
{
    public static void ElementsModified(this CombatHistory combatHistory, ICombatState combatState, Elements amount,
        Player player)
    {
        combatHistory.Add(combatState, new ElementsModifiedEntry(amount, player, combatState.RoundNumber,
            combatState.CurrentSide,
            combatHistory));
    }

    // TODO history for runes crafted
}