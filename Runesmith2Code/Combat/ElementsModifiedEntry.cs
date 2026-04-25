using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Combat;

public class ElementsModifiedEntry(
    Elements amount,
    Player player,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history) : CombatHistoryEntry(player.Creature, roundNumber, currentSide, history)
{
    public Elements Amount { get; } = amount;

    public override string Description
    {
        get
        {
            var left = $"{Actor.Player.Character.Id.Entry} {(Amount.Total < 0 ? "lost" : "gained")} ";

            string[] arr =
            [
                $"{(Amount.Ignis != 0 ? $"{Amount.Ignis} ignis" : null)}",
                $"{(Amount.Terra != 0 ? $"{Amount.Terra} terra" : null)}",
                $"{(Amount.Aqua != 0 ? $"{Amount.Aqua} aqua" : null)}"
            ];

            return $"{left} {string.Join(", ", arr.Where(s => !string.IsNullOrEmpty(s)))}";
        }
    }
}