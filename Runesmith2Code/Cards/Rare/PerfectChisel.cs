#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class PerfectChisel : Runesmith2Card
{
    public PerfectChisel() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithVars(new PotencyVar(3).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Enhance);
        WithTip(new TooltipSource(_ => HoverTipFactory.FromPower<AmpPower>()));
        WithTags(RunesmithEnum.Chisel);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue != null)
            await RuneCmd.AddPotency(choiceContext, runeQueue.Runes, Owner, play,
                DynamicVars[PotencyVar.defaultName].BaseValue);
    }
}