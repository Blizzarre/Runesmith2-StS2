#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Accelerate : Runesmith2Card
{
    private const string ExtraCardsKey = "ExtraCards";

    public Accelerate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(7, 2);
        WithCards(1);
        WithVar(ExtraCardsKey, 2, 1);

        WithTip(RunesmithHoverTip.Break);
    }

    protected override bool ShouldGlowGoldInternal => HasRune();

    public override RuneBreakType RuneBreakType => RuneBreakType.Oldest;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        if (HasRune())
        {
            await RuneCmd.BreakOldest(choiceContext, Owner);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue + DynamicVars[ExtraCardsKey].IntValue, Owner);
        }
        else
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        }
    }
}