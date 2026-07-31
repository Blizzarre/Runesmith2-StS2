#region

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Refine : Runesmith2Card
{
    public Refine() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithVars(new DynamicVar("Amount", 2), new CardsVar(1));
        WithTip(RunesmithHoverTip.Enhance);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceSelectionPrompt, DynamicVars.Cards.IntValue),
            card => card.CanEnhance(),
            this
        );
        
        await RunesmithCardCmd.Enhance(choiceContext, Owner, cards, play,
            ResolveEnergyXValue() * DynamicVars["Amount"].IntValue);
    }
}