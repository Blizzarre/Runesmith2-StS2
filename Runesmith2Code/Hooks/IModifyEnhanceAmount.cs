using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyEnhanceAmount
{
    public int ModifyEnhanceAmount(Player player, int amount);
}

public interface IAfterModifyingEnhanceAmount
{
    public Task AfterModifyingEnhanceAmount(int modifiedEnhance, CardModel? cardSource, CardPlay? cardPlay);
}