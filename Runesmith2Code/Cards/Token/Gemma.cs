#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Gemma : Runesmith2Card
{
    public Gemma() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithVars(new EnhanceByVar(1), new CardsVar(1));
        WithTip(RunesmithHoverTip.Enhance);
        WithCostUpgradeBy(-1);
    }

    private int EnhanceDiff
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public void SetEnhanceBy(int amount)
    {
        EnhanceDiff = amount - DynamicVars[EnhanceByVar.defaultName].IntValue;
        DynamicVars[EnhanceByVar.defaultName].BaseValue = amount;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars[EnhanceByVar.defaultName].BaseValue += EnhanceDiff;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cards = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceSelectionPrompt, DynamicVars.Cards.IntValue),
            card => card.CanEnhance(), this
        );
        var enhanceBy = DynamicVars[EnhanceByVar.defaultName].IntValue;
        await RunesmithCardCmd.Enhance(choiceContext, Owner, cards, play, enhanceBy);
    }
}