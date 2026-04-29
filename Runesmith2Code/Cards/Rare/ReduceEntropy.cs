#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class ReduceEntropy : Runesmith2Card
{
    public ReduceEntropy() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithPower<IceColdPower>(4, 1);
        WithVar("SelfIceColdPower", 2);
        WithBlock(11, 4);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (CombatState != null)
            foreach (var hittableEnemy in CombatState.HittableEnemies)
                await CommonActions.Apply<IceColdPower>(choiceContext, hittableEnemy, this);

        await CommonActions.CardBlock(this, play);

        await CommonActions.ApplySelf<IceColdPower>(choiceContext, this, DynamicVars["SelfIceColdPower"].IntValue);
    }
}