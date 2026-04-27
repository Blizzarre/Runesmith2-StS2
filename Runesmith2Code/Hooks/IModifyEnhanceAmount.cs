#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

#endregion

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyEnhanceAmount
{
    public int ModifyEnhanceAmount(Player player, int amount, CardModel? cardSource);
}

public interface IAfterModifyingEnhanceAmount
{
    public Task AfterModifyingEnhanceAmount(int modifiedEnhance, CardModel? cardSource, CardPlay? cardPlay);
}