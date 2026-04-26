#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class CompleteCircuit : Runesmith2Card
{
    public CompleteCircuit() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(6, 2);
        WithPower<AmpPower>(2, 1);
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
        var amount = DynamicVars.Power<AmpPower>().BaseValue * hittableEnemies.Count;
        await CommonActions.ApplySelf<AmpPower>(choiceContext, this, amount);
    }
}