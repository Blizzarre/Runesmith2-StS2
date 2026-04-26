#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Stabilize : Runesmith2Card
{
    public Stabilize() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(6, 2);
        WithVar(new ChargeVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Charge);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue != null)
        {
            var amount = DynamicVars[ChargeVar.defaultName].IntValue;
            foreach (var rune in runeQueue.Runes) RuneCmd.SetCharge(choiceContext, rune, amount);
        }
    }
}