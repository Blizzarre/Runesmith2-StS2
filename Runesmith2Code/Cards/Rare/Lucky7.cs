#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Extensions;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Lucky7 : Runesmith2Card
{
    public Lucky7() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(35,14);
    }
    
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override bool IsPlayable {
        get
        {
            var runeQueue = Owner.PlayerCombatState?.RuneQueue();
            return runeQueue != null && runeQueue.Runes.Count(r => r.ChargeVal > 0) == 7;
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}