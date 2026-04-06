using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

namespace Runesmith2.Runesmith2Code.Cards.Basic;

public class Fortify() : Runesmith2Card(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        RunesmithHoverTipFactory.Static(RunesmithHoverTip.Enhance)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        // Select enhance
        if (IsUpgraded)
        {
            var cardModel = (await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceSelectionPrompt, 1),
                card => card.IsEnhanceable(),
                this
            )).FirstOrDefault();
            if (cardModel != null)
            {
                await RunesmithCardCmd.Enhance(choiceContext, cardModel, 1);
            }
            return;
        }
        
        // Random enhance
        var pile = PileType.Hand.GetPile(Owner);
        var cardModel2 = Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
        if (cardModel2 != null)
        {
            await RunesmithCardCmd.Enhance(choiceContext, cardModel2, 1);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}