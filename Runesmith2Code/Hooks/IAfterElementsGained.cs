using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IAfterElementsGained
{
    public Task AfterElementsGained(ICombatState combatState, Elements amount, Player player,
        CardPlay? cardPlay = null);
}