#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class DryIce : Runesmith2Card
{
    public DryIce() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithBlock(4, 2);
        WithVar(new AquaVar(1));
        WithPower<IceColdPower>(2, 1);
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await CommonActions.CardBlock(this, play);
        await CommonActions.Apply<IceColdPower>(choiceContext, play.Target, this);
        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
    }
}