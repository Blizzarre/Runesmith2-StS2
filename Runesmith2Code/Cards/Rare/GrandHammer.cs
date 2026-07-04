#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class GrandHammer : Runesmith2Card
{
    public GrandHammer() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(10);
        WithTags(RunesmithTags.Hammer);
        WithVar(new EnhanceByVar(1).WithUpgrade(1));
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .SpawningHitVfxOnEachCreature()
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await RunesmithCardCmd.Enhance(choiceContext, Owner,
            PileType.Hand.GetPile(Owner).Cards.Where(c => c.CanEnhance()), play,
            DynamicVars[EnhanceByVar.defaultName].IntValue);
    }
}