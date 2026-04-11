using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

namespace Runesmith2.Runesmith2Code.Cards.Basic;

public class Fortify : Runesmith2Card
{
    public Fortify() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(6, 2);
        WithTip(RunesmithHoverTip.Enhance);
    }

    public override bool GainsBlock => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        // Select enhance   
        if (IsUpgraded)
        {
            // TODO create custom selection screen (like upgrade) to show enhanced value
            var card = (await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceSelectionPrompt, 1),
                card => card.IsEnhanceable(),
                this
            )).FirstOrDefault();
            if (card != null)
            {
                await RunesmithCardCmd.Enhance(choiceContext, card, 1);
            }
            return;
        }
        
        // Random enhance
        var pile = PileType.Hand.GetPile(Owner);
        var randomCard = Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
        if (randomCard != null)
        {
            await RunesmithCardCmd.Enhance(choiceContext, randomCard, 1);
        }
    }
}