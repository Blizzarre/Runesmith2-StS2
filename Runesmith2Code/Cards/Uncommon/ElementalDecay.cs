#region

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class ElementalDecay : Runesmith2Card
{
    public ElementalDecay() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(2);
        WithCostUpgradeBy(-1);
        WithTip(CardKeyword.Exhaust);
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cards = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, DynamicVars.Cards.IntValue),
            null,
            this
        )).ToList();

        var totalCost = new Elements(cards.Select(c => c.EnergyCost.GetAmountToSpend())
            .Aggregate(0, (a, b) => a + b));

        if (totalCost.Total > 0)
        {
            await Cmd.CustomScaledWait(0.1f, 0.2f);
            await RunesmithPlayerCmd.GainElements(totalCost, Owner, play);
        }

        foreach (var card in cards) await CardCmd.Exhaust(choiceContext, card);
    }
}