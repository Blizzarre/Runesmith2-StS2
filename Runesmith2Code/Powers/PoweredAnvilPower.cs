#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Hooks;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class PoweredAnvilPower : Runesmith2Power, IModifyEnhanceAmount, IAfterModifyingEnhanceAmount
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;


    public int ModifyEnhanceAmount(Player player, int amount, CardModel? cardSource)
    {
        if (player != Owner.Player || amount < 0) return amount;
        return amount + Amount;
    }

    public Task AfterModifyingEnhanceAmount(int modifiedEnhance, CardModel? cardSource, CardPlay? cardPlay)
    {
        Flash();
        return Task.CompletedTask;
    }
}