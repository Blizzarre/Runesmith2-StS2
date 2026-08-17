#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class WhiteBalance : Runesmith2Card
{
    public WhiteBalance() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(8, 3);
        WithVars(new ElementsVar(1));
        WithEnergy(1);
        WithTip(RunesmithHoverTip.Elements);
        WithEnergyTip();
    }

    protected override bool ShouldGlowGoldInternal => HasAllElements();

    private bool HasAllElements()
    {
        var elements = Owner.PlayerCombatState?.GetElements() ?? new Elements(0);
        return elements is { Ignis: > 0, Terra: > 0, Aqua: > 0 };
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        if (HasAllElements())
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        else
            await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
    }
}