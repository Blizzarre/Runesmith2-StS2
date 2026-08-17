#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class HammerTornado : Runesmith2Card
{
    private const string CalculatedHitsKey = "CalculatedHits";

    public HammerTornado() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
        WithVar(new CardsVar(1));
        WithCalculatedVar(CalculatedHitsKey, 1, (card, _) =>
        {
            var runeQueue = card.Owner.PlayerCombatState?.GetRuneQueue();
            return runeQueue?.Runes.DistinctBy(r => r.Id).Count() ?? 0;
        });
        WithTags(RunesmithTags.Hammer);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount((int)((CalculatedVar)DynamicVars[CalculatedHitsKey]).Calculate(play.Target))
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
    }
}