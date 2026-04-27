#region

using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

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
        
        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceAndUpgradeSelectionPrompt,
                DynamicVars.Cards.IntValue),
            card => card.CanEnhance() || card.IsUpgradable,
            this
        );

        foreach (var card in cards)
        {
            if (card.IsUpgradable)
            {
                CardCmd.Upgrade(card);
            }
            if (card.CanEnhance())
            {
                await RunesmithCardCmd.Enhance(choiceContext, Owner, card, play,
                    DynamicVars[EnhanceByVar.defaultName].IntValue);
            }
        }
    }
}