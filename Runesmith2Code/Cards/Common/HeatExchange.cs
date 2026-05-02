#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class HeatExchange : Runesmith2Card
{
    public HeatExchange() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithCalculatedDamage(6, 1, (card, _) => card.Owner.PlayerCombatState?.Elements().Ignis ?? 0,
            ValueProp.Move, 0, 1);
        WithVar("IgnisLoss", 1);
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;
        var hittableEnemies = CombatState.HittableEnemies;
        foreach (var enemy in hittableEnemies)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        await RunesmithPlayerCmd.LoseElements(Elements.WithIgnis(DynamicVars["IgnisLoss"].IntValue), Owner);
    }
}