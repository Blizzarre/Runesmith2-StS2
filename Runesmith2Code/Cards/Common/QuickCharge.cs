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

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class QuickCharge : Runesmith2Card
{
    public QuickCharge() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithVar(new ChargeGainVar(2).WithUpgrade(1));
        WithPower<AmpPower>(1, 1);
        WithTip(RunesmithHoverTip.Charge);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (HasRune())
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            var rune = RuneCmd.ChargeOldest(choiceContext, Owner, DynamicVars[ChargeGainVar.defaultName].IntValue);
            await RuneCmd.Passive(choiceContext, Owner, rune, 1);
        }
        else
        {
            await CommonActions.ApplySelf<AmpPower>(choiceContext, this);
        }
    }
}