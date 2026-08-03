#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class EnergySink : Runesmith2Card
{
    public EnergySink() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("ChargeLoss", 1);
        WithTip(RunesmithHoverTip.Charge);
        WithTips(static (card) =>
        {
            if (!card.IsUpgraded) return [RunesmithHoverTipFactory.Static(RunesmithHoverTip.Potency)];
            return [];
        });
        WithEnergyTip();
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var runeQueue = Owner.PlayerCombatState?.GetRuneQueue();
        if (runeQueue != null && runeQueue.HasAny())
        {
            var energyToGain = runeQueue.Runes.Count(r => r.ChargeVal > 0);
            RuneCmd.ChargeAll(choiceContext, Owner, -DynamicVars["ChargeLoss"].IntValue);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
            await PlayerCmd.GainEnergy(energyToGain, Owner);
        }
    }
}