using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyElementsGain
{
    public Elements ModifyElementsGain(Player player, Elements amount, ValueProp props, CardModel? cardSource);
}

public interface IAfterModifyingElementsGain
{
    public Task AfterModifyingElementsGain();
}