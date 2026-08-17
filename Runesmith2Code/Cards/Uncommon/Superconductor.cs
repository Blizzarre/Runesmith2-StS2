#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Superconductor : Runesmith2Card
{
    public Superconductor() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithTip(RunesmithHoverTip.Break);
        WithTip(RunesmithHoverTip.Charge);
        WithEnergyTip();
    }

    protected override bool ShouldGlowGoldInternal => HasRune();

    public override RuneBreakType RuneBreakType => RuneBreakType.Oldest;

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("HasRune", HasRune());
        description.Add("EnergyGain", GetOldestRuneCharge());
    }

    private int GetOldestRuneCharge()
    {
        if (!IsInCombat) return 0;

        var runeQueue = Owner.PlayerCombatState?.GetRuneQueue();
        if (runeQueue != null && runeQueue.HasAny()) return runeQueue.Runes[0].ChargeVal;

        return 0;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        var rune = await RuneCmd.BreakOldest(choiceContext, Owner);
        var energyToGain = rune?.ChargeVal ?? 0;
        if (energyToGain > 0) await PlayerCmd.GainEnergy(energyToGain, Owner);
    }
}