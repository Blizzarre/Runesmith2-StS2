#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class AdamantiumHammer : Runesmith2Card, IAfterCardEnhanced
{
    public AdamantiumHammer() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithTip(RunesmithHoverTip.Enhance);
        WithTags(RunesmithTags.Hammer);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }
    
    public Task AfterCardEnhanced(PlayerChoiceContext choiceContext, CardModel card, Player applier, CardPlay? cardPlay, int enhanceAmount)
    {
        if (applier != Owner || enhanceAmount <= 0 || card == this) return Task.CompletedTask;

        RunesmithCardCmd.AddEnhance([this], enhanceAmount);
        return Task.CompletedTask;
    }
}