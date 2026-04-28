#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class ElementalDecay : Runesmith2Card
{
    public ElementalDecay() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1, 1);
        WithTip(new TooltipSource(_ => HoverTipFactory.FromKeyword(CardKeyword.Exhaust)));
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, DynamicVars.Cards.IntValue),
            null,
            this
        );

        var totalCost = cards.Select(c => c.EnergyCost.GetAmountToSpend()).Aggregate(0, (a, b) => a + b);

        if (totalCost > 0)
        {
            await Cmd.CustomScaledWait(0.1f, 0.2f);
            await RunesmithPlayerCmd.GainElements(new Elements(totalCost), Owner, play);
        }
    }
}