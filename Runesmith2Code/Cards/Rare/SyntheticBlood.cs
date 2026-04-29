#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class SyntheticBlood : Runesmith2Card
{
    public SyntheticBlood() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVar("Amount", 2, 1);
        WithVars(new ElementsVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
        await CommonActions.ApplySelf<SyntheticBloodPower>(choiceContext, this,  DynamicVars["Amount"].IntValue);
    }
}