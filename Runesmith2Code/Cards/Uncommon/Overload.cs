#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Overload : Runesmith2Card
{
    public Overload() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar(new ChargeGainVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Charge);
        WithTip(RunesmithHoverTip.Break);
    }

    public override RuneBreakType RuneBreakType => RuneBreakType.Oldest;

    protected override bool ShouldGlowGoldInternal => HasRune();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var rune = RuneCmd.ChargeOldest(choiceContext, Owner, DynamicVars[ChargeGainVar.defaultName].IntValue);

        if (rune is { ChargeVal: > 0 })
        {
            var count = rune.ChargeVal;
            await Cmd.CustomScaledWait(0.1f, 0.2f);
            await RuneCmd.Passive(choiceContext, Owner, rune, count, false);

            await Cmd.CustomScaledWait(0.15f, 0.25f);
            await RuneCmd.Break(choiceContext, Owner, rune);
        }
    }
}