#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class DuctTapePower : Runesmith2Power, IAfterCardEnhanced, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override object InitInternalData() => new Data();
    
    private class Data
    {
        public int CardsStasisThisTurn;
    }
    
    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;
        GetInternalData<Data>().CardsStasisThisTurn = 0;
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }

    public Task AfterCardEnhanced(PlayerChoiceContext choiceContext, CardModel card, Player applier, CardPlay? cardPlay, int enhanceAmount)
    {
        if (applier != Owner.Player || card.IsStasis() || enhanceAmount <= 0) return Task.CompletedTask;
        // Skip stasis if it's a card that's enhancing itself and won't go into your combat piles
        if (cardPlay != null && card == cardPlay.Card && !cardPlay.ResultPile.IsCombatPile()) return Task.CompletedTask;
        var data = GetInternalData<Data>();
        if (data.CardsStasisThisTurn >= Amount) return Task.CompletedTask;
        ++data.CardsStasisThisTurn;
        Flash();
        RunesmithCardCmd.Stasis(card);
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }

    public string GetSecondAmount()
    {
        var data = GetInternalData<Data>();
        return $"{Math.Max(Amount - data.CardsStasisThisTurn, 0)}";
    }
}