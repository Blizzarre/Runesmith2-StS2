#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

#endregion

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyPotencyAdditive
{
    public decimal ModifyPotencyAdditive(Player player, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay);
}

public interface IModifyPotencyMultiplicative
{
    public decimal ModifyPotencyMultiplicative(Player player, decimal block, ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay);
}

public interface IAfterModifyingPotency
{
    public Task AfterModifyingPotency();
}