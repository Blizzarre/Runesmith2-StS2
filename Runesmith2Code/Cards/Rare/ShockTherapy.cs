#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class ShockTherapy : Runesmith2Card
{
    private const string CalculatedHitsKey = "CalculatedHits";
    
    public ShockTherapy() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(5, 2);
        WithCards(2);
        WithCalculatedVar(CalculatedHitsKey, 0, GetPrecalculatedHits, 1);
        WithTip(RunesmithHoverTip.Stasis);
    }
    
    private static decimal GetPrecalculatedHits(CardModel card, Creature? _)
    {
        var toStasisCards = card.DynamicVars.Cards.BaseValue;
        var isStasisCards = PileType.Hand.GetPile(card.Owner).Cards.Count(c => c.IsStasis());
        var canStasisCards = PileType.Hand.GetPile(card.Owner).Cards.Count(c => c.CanStasis());
        return isStasisCards + Math.Min(toStasisCards, canStasisCards);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cards = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(RunesmithCardSelectorPrefs.StasisSelectionPrompt, DynamicVars.Cards.IntValue),
            card => card.CanStasis(),
            this
        );
        foreach (var card in cards) RunesmithCardCmd.Stasis(card);
        
        var stasisCardCount = PileType.Hand.GetPile(Owner).Cards.Count(c => c.IsStasis());

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .WithHitCount(stasisCardCount)
            .WithHitFx("vfx/vfx_attack_lightning")
            .TargetingAllOpponents(CombatState!)
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
    }
}