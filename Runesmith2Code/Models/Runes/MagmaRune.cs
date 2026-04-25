using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Basic;
using Runesmith2.Runesmith2Code.Nodes.Runes;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Models.Runes;

public class MagmaRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 4;
    public override int ChargeVal { get; set; } = 3;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.EndTurn;
    public override (bool, bool) ShowTopLabel => (true, true);
    public override (decimal, decimal) TopValue => (CalculatedPassiveVal, CalculatedPassiveVal);
    public override (Color, Color, Color) TopLabelColor => NRune.BlueFontColor;
    public override decimal CalculatedPassiveVal => PassiveVal / 2;
    public override decimal CalculatedBreakVal => BreakVal / 2;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Flamma>();

    public override async Task BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            await ApplyFireDamage(choiceContext, PassiveVal);
            await GainBlock(choiceContext, CalculatedPassiveVal);
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
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
            PlayPassiveSfx();
            await CreatureCmd.Damage(choiceContext, target, amount, ValueProp.Unpowered, Owner.Creature);
        }
    }
    
    private async Task GainBlock(PlayerChoiceContext _, decimal amount)
    {
        await CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Unpowered, null);
    }
}