#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class DoubleUp : Runesmith2Card
{
    public DoubleUp() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(10);
        WithCards(1, 1);
        WithVar(new EnhanceByVar(1));
        WithTip(RunesmithHoverTip.Enhance);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        var cards = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceAndUpgradeSelectionPrompt,
                DynamicVars.Cards.IntValue),
            card => card.CanEnhance() || card.IsUpgradable,
            this
        )).ToList();

        foreach (var card in cards.Where(card => card.IsUpgradable)) CardCmd.Upgrade(card);
        await RunesmithCardCmd.Enhance(choiceContext, Owner, cards.Where(c => c.CanEnhance()), play,
            DynamicVars[EnhanceByVar.defaultName].IntValue);
    }
}