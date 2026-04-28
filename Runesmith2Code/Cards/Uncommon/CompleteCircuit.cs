#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class CompleteCircuit : Runesmith2Card
{
    public CompleteCircuit() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(5, 4);
        WithVar(new ChargeGainVar(1));
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var hittableEnemies = CombatState.HittableEnemies;
        foreach (var enemy in hittableEnemies)
            VfxCmd.PlayOnCreature(enemy, "vfx/vfx_attack_lightning");

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);

        var amount = hittableEnemies.Count;
        RuneCmd.ChargeAll(choiceContext, Owner, DynamicVars[ChargeGainVar.defaultName].IntValue * amount);
    }
}