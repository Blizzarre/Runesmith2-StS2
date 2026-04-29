#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class AdamantiumHammer : Runesmith2Card, IAfterCardEnhanced
{
    public AdamantiumHammer() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(10, 4);
        WithTip(RunesmithHoverTip.Enhance);
        WithTags(RunesmithEnum.Hammer);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }


    public Task AfterCardEnhanced(PlayerChoiceContext choiceContext, CardModel card, int enhanceAmount)
    {
        if (card.Owner != Owner || enhanceAmount <= 0 || card == this) return Task.CompletedTask;

        this.AddEnhance(enhanceAmount);
        return Task.CompletedTask;
    }
}