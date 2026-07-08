#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class ConduitPower : Runesmith2Power, IAfterCardEnhanced
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;
    
    
    public Task AfterCardEnhanced(PlayerChoiceContext choiceContext, CardModel card, Player applier, CardPlay? cardPlay, int enhanceAmount)
    {
        if (Owner.Player == null || applier != Applier?.Player) return Task.CompletedTask;

        var cards = PileType.Hand.GetPile(Owner.Player).Cards.Where(c => c.CanEnhance()).ToList();
        if (cards.Count == 0) return Task.CompletedTask;
        
        var randomCards = cards.StableShuffle(Owner.Player.RunState.Rng.CombatCardSelection);
        var targetCards = randomCards.Take(Amount);
        RunesmithCardCmd.AddEnhance(targetCards, enhanceAmount);
        
        return Task.CompletedTask;
    }
}