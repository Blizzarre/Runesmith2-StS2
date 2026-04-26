using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class RuneHurl : Runesmith2Card
{
    public RuneHurl() : base(0, CardType.Attack, CardRarity.Common, TargetType.Self)
    {
        WithDamage(10, 4);
        WithVar(new CardsVar(1));
        WithTip(RunesmithHoverTip.Break);
    }

    protected override bool ShouldGlowGoldInternal => HasRune();

    public override TargetType TargetType => HasRune() ? TargetType.AnyEnemy : TargetType.Self;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (HasRune())
        {
            ArgumentNullException.ThrowIfNull(play.Target);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);
            await RuneCmd.BreakOldest(choiceContext, Owner);
        }
        else
        {
            await CommonActions.Draw(this, choiceContext);
        }
    }
}