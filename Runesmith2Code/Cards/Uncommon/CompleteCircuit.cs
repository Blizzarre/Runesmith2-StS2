#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class CompleteCircuit : Runesmith2Card
{
    public CompleteCircuit() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(5, 4);
        WithVar(new ChargeVar(1));
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
        for (var i = 0; i < amount; i++)
        {
            RuneCmd.ChargeAll(choiceContext, Owner, DynamicVars[ChargeVar.defaultName].IntValue);
        }
    }
}