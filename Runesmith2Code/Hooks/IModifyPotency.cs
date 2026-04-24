using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IOnModifyPotencyAdditive
{
    public decimal ModifyPotencyAdditive(Player player, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay);
}

public interface IOnModifyPotencyMultiplicative
{
    public decimal ModifyPotencyMultiplicative(Player player, decimal block, ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay);
}