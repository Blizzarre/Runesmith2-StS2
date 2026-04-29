#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class ChargedHammer : Runesmith2Card
{
    public ChargedHammer() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(12, 2);
        WithVar(new ChargeGainVar(1).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Charge);
        WithTags(RunesmithEnum.Hammer);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        VfxCmd.PlayOnCreature(play.Target, "vfx/vfx_attack_lightning");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);

        RuneCmd.ChargeAll(choiceContext, Owner, DynamicVars[ChargeGainVar.defaultName].IntValue);

        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue != null && runeQueue.HasAny())
            foreach (var rune in runeQueue.Runes)
            {
                await RuneCmd.Passive(choiceContext, rune);
                await Cmd.CustomScaledWait(0.1f, 0.2f);
            }
    }
}