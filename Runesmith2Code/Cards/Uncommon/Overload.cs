using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Overload : Runesmith2Card
{
    public Overload() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar(new ChargeVar(2).WithUpgrade(1));
        WithVar(new DynamicVar("Threshold", 5));
        WithTip(RunesmithHoverTip.Charge);
        WithTip(RunesmithHoverTip.Break);
    }

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (!IsInCombat) return false;
            var runeQueue = Owner.PlayerCombatState?.RuneQueue();
            if (runeQueue == null || !runeQueue.HasAny()) return false;
            var rune = runeQueue.Runes[0];
            return rune.ChargeVal + DynamicVars[ChargeVar.defaultName].BaseValue >= DynamicVars["Threshold"].BaseValue;
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var rune = RuneCmd.ChargeOldest(choiceContext, Owner, DynamicVars[ChargeVar.defaultName].IntValue);

        if (rune != null && rune.ChargeVal >= DynamicVars["Threshold"].IntValue)
        {
            var count = rune.ChargeVal;
            for (var i = 0; i < count; i++)
            {
                await Cmd.CustomScaledWait(0.1f, 0.15f);
                await RuneCmd.Passive(choiceContext, rune);
            }
            await Cmd.CustomScaledWait(0.1f, 0.15f);
            await RuneCmd.Break(choiceContext, Owner, rune);
        }
    }
}