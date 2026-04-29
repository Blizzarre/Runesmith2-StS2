#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class GrandSlam : Runesmith2Card
{
    public GrandSlam() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(10);
        WithCalculatedVar("EnhanceBy", 1, GetEnhanceBonus, 1);
        WithTip(RunesmithHoverTip.Enhance);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(CombatState)
            .SpawningHitVfxOnEachCreature()
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await RunesmithCardCmd.Enhance(choiceContext, Owner,
            PileType.Hand.GetPile(Owner).Cards.Where(c => c.CanEnhance()), play,
            DynamicVars["EnhanceByBase"].IntValue);
    }
}