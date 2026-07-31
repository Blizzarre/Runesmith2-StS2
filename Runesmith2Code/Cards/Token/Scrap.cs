#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
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
        WithVar(BreakCountKey, 1);
        WithTip(RunesmithHoverTip.Break);
    }

    public override RuneBreakType RuneBreakType => RuneBreakType.Oldest;

    protected override bool ShouldGlowGoldInternal => HasRune();

    protected override bool IsPlayable => HasRune();

    private bool ShouldReturnNextTurn { get; set; } = false;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!ShouldReturnNextTurn && !IsUpgraded)
            ShouldReturnNextTurn = true;
        
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

    protected override CardLocation GetResultLocationForCardPlay()
    {
        var locationForCardPlay = base.GetResultLocationForCardPlay();
        if (IsUpgraded && locationForCardPlay.pileType == PileType.Discard)
            locationForCardPlay.pileType = PileType.Hand;
        return locationForCardPlay;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == Owner &&
            CombatManager.Instance.History.CardPlaysFinished.Any(e =>
                e.HappenedLastPlayerTurn(Owner) && e.CardPlay.Card == this && ShouldReturnNextTurn))
        {
            ShouldReturnNextTurn = false;
            if (Pile is not { Type: PileType.Hand })
            {
                await CardPileCmd.Add(this, PileType.Hand);
            }
        }
    }
}