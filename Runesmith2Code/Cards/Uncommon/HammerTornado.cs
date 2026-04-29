#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class HammerTornado : Runesmith2Card
{
    private const string CalculatedHitsKey = "CalculatedHits";

    public HammerTornado() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 4);
        WithVar(new CardsVar(1));
        WithCalculatedVar(CalculatedHitsKey, 1, (card, _) =>
        {
            var runeQueue = card.Owner.PlayerCombatState?.RuneQueue();
            return runeQueue is { Runes.Count: > 0 } ? runeQueue.Runes[0].ChargeVal : 0;
        });
        WithTip(RunesmithHoverTip.Charge);
        WithTip(RunesmithHoverTip.Break);
        WithTags(RunesmithEnum.Hammer);
    }

    public override RuneBreakType RuneBreakType => RuneBreakType.Oldest;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount((int)((CalculatedVar)DynamicVars[CalculatedHitsKey]).Calculate(play.Target))
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);

        await RuneCmd.BreakOldest(choiceContext, Owner);
    }
}