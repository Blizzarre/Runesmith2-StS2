#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Flowstate : Runesmith2Card
{
    public Flowstate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<AmpPower>(4, 2);
        WithVar(new AquaVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<AmpPower>(choiceContext, this);
        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
    }
}