#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class MakeshiftArmor : Runesmith2Card
{
    public MakeshiftArmor() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 3);
        WithTip(RunesmithHoverTip.Break);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        if (HasRune())
        {
            await Cmd.CustomScaledWait(0.1f, 0.2f);
            await CommonActions.CardBlock(this, play);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
            await RuneCmd.BreakOldest(choiceContext, Owner);
        }
    }
}