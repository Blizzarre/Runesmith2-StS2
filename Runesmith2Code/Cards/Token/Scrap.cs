#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Scrap : Runesmith2Card
{
    private const string BreakCountKey = "BreakCount";
    
    public Scrap() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain);
        WithVar(BreakCountKey, 1, 1);
        WithTip(RunesmithHoverTip.Break);
    }

    public override RuneBreakType RuneBreakType => RuneBreakType.Oldest;

    protected override bool ShouldGlowGoldInternal => HasRune();

    protected override bool IsPlayable => HasRune();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (HasRune())
        {
            var count = DynamicVars[BreakCountKey].IntValue;
            for (var i = 0; i < count - 1; i++)
            {
                await RuneCmd.BreakOldest(choiceContext, Owner, false);
                await Cmd.CustomScaledWait(0.15f, 0.25f);
            }
            await RuneCmd.BreakOldest(choiceContext, Owner);
        }
        await Cmd.Wait(0.20f);
    }



    protected override (PileType, CardPilePosition) GetResultPileTypeAndPositionForCardPlay()
    {
        var (pileType, cardPilePosition) = base.GetResultPileTypeAndPositionForCardPlay();
        return pileType != PileType.Discard ? (pileType, cardPilePosition) : (PileType.Hand, CardPilePosition.Bottom);
    }
}