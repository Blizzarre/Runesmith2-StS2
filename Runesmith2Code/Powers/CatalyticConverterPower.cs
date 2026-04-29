#region

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class CatalyticConverterPower : Runesmith2Power, IModifyElementsGain, IAfterModifyingElementsGain
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public Elements ModifyElementsGain(Player player, Elements amount, ValueProp props, CardModel? cardSource)
    {
        if (player != Owner.Player) return amount;
        return new Elements(
            amount.Ignis > 0 ? amount.Ignis + Amount : 0,
            amount.Terra > 0 ? amount.Terra + Amount : 0,
            amount.Aqua > 0 ? amount.Aqua + Amount : 0
        );
    }

    public Task AfterModifyingElementsGain()
    {
        Flash();
        return Task.CompletedTask;
    }
}