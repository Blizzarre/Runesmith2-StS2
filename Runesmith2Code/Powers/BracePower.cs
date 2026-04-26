#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class BracePower : Runesmith2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private class Data
    {
        public CardModel? CardToModify;
        public int AmountWhenCardPlayed;
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (cardPlay.Card.Owner != Owner.Player) return Task.CompletedTask;
        var internalData = GetInternalData<Data>();
        if (internalData.CardToModify != null || !card.GainsBlock) return Task.CompletedTask;
        internalData.CardToModify = card;
        internalData.AmountWhenCardPlayed = Amount;
        return Task.CompletedTask;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource != null)
        {
            if (cardSource.Owner.Creature != Owner) return 0m;
        }
        else if (Owner != target)
        {
            return 0m;
        }

        if (!props.IsPoweredCardOrMonsterMoveBlock()) return 0m;
        return Amount;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var internalData = GetInternalData<Data>();
        var card = cardPlay.Card;
        if (card == internalData.CardToModify)
            await PowerCmd.ModifyAmount(choiceContext, this, -internalData.AmountWhenCardPlayed, null, null);
    }
}