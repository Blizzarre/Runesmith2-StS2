using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class EarthShield : Runesmith2Card
{
    public EarthShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 3);
        WithElementsVar(new TerraVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Elements);
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await RunesmithPlayerCmd.GainElements(Elements.WithTerra(DynamicVars[TerraVar.defaultName].IntValue), Owner);
    }

}