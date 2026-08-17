#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class ParticleAccelerator : Runesmith2Card
{
    public ParticleAccelerator() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVar("Amount", 2, 1);
        WithVar("TriggerCount", 1);
        WithTip(RunesmithHoverTip.Craft);
        WithTip(RunesmithHoverTip.Charge);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var existingPower = Owner.Creature.GetPower<ParticleAcceleratorPower>() ??
                            (ParticleAcceleratorPower)ModelDb.Power<ParticleAcceleratorPower>().ToMutable();
        existingPower.IncrementTriggerCount(DynamicVars["TriggerCount"].IntValue);
        await PowerCmd.Apply(choiceContext, existingPower, Owner.Creature, DynamicVars["Amount"].IntValue,
            Owner.Creature, this);
    }
}