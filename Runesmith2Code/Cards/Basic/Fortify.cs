using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

namespace Runesmith2.Runesmith2Code.Cards.Basic;

public class Fortify : Runesmith2Card
{
    public Fortify() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(6);
        WithVar(new EnhanceByVar(1).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Enhance);
    }

    public override bool GainsBlock => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

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
            await RunesmithCardCmd.Enhance(choiceContext, Owner, card, play,
                DynamicVars[EnhanceByVar.defaultName].IntValue);
        }
    }
}