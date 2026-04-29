#region

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Common;
using Runesmith2.Runesmith2Code.Nodes.Runes;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Deal damage, gain Block
public class MagmaRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 4;
    public override int ChargeVal { get; set; } = 3;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.EndTurn;
    public override (bool, bool) ShowTopLabel => (true, true);
    public override (decimal, decimal) TopValue => (CalculatedPassiveVal, CalculatedBreakVal);
    public override (Color, Color, Color) TopLabelColor => NRune.BlueFontColor;
    public override decimal CalculatedPassiveVal => (int)(PassiveVal / 2);
    public override decimal CalculatedBreakVal => (int)(BreakVal / 2);

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Magma>();

    public override async Task BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            PlayPassiveSfx();
            Trigger();
            await ApplyFireDamage(choiceContext, PassiveVal);
            await GainBlock(choiceContext, CalculatedPassiveVal);
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        PlayPassiveSfx();
        await ApplyFireDamage(choiceContext, BreakVal);
        await GainBlock(choiceContext, CalculatedBreakVal);
    }

    private async Task ApplyFireDamage(PlayerChoiceContext choiceContext, decimal amount)
    {
        var list = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsHittable).ToList();
        if (list.Count == 0) return;

        var target = Owner.RunState.Rng.CombatTargets.NextItem(list);
        if (target != null)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(target));
            await CreatureCmd.Damage(choiceContext, target, amount, ValueProp.Unpowered, Owner.Creature);
        }
    }

    private async Task GainBlock(PlayerChoiceContext _, decimal amount)
    {
        await CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Unpowered, null);
    }
}