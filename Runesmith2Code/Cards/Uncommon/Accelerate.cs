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
    private const string CardsKey = "Cards";

    public Accelerate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(7, 2);
        WithCalculatedVar(CardsKey, 0, (card, _) =>
        {
            var runeQueue = card.Owner.PlayerCombatState?.RuneQueue();
            return runeQueue is { Runes.Count: > 0 } ? runeQueue.Runes[0].ChargeVal : 0;
        }, 1);

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
            var cardsToDraw = (int)((CalculatedVar)DynamicVars[CardsKey]).Calculate(play.Target);
            await RuneCmd.BreakOldest(choiceContext, Owner);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
            await CardPileCmd.Draw(choiceContext, cardsToDraw, Owner);
        }
        else
        {
            var cardsToDraw = (int)((CalculatedVar)DynamicVars[CardsKey]).Calculate(play.Target);
            await CardPileCmd.Draw(choiceContext, cardsToDraw, Owner);
        }
    }
}