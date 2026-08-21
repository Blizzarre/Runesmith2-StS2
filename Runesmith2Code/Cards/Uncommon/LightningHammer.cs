#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class LightningHammer : Runesmith2Card
{
    public LightningHammer() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(10, 3);
        WithTip(RunesmithHoverTip.Stasis);
        WithTags(RunesmithTags.Hammer);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override bool PreserveStasis => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        VfxCmd.PlayOnCreature(play.Target, "vfx/vfx_attack_lightning");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
    }

    public override Task BeforeCombatStart()
    {
        this.SetStasis(true);
        return Task.CompletedTask;
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card == this) this.SetStasis(true);
        return Task.CompletedTask;
    }

    protected override CardLocation GetResultLocationForCardPlay()
    {
        var location = base.GetResultLocationForCardPlay();
        if (CombatState == null) return location;

        var teammates = CombatState.GetTeammatesOf(Owner.Creature)
            .Where(c => c is { IsAlive: true, IsPlayer: true } && c.Player != Owner).ToList();
        if (teammates.Count == 0) return location;

        var player = Owner.RunState.Rng.CombatTargets.NextItem(teammates)?.Player!;
        location.player = player;

        if (location.pileType == PileType.Discard)
        {
            location.pileType = PileType.Hand;
            location.position = CardPilePosition.Bottom;
        }

        return location;
    }
}