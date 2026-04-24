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
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Models.Runes;

public class FlammaRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 4;
    public override int ChargeVal { get; set; } = 2;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.EndTurn;
    public override (bool, bool) ShowTopLabel => (false, false);
    public override (decimal, decimal) TopValue => (0, 0);
    public override (bool, bool) ShowBottomLabel => (true, true);
    public override (decimal, decimal) BottomValue => (PassiveVal, BreakVal);

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
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await ApplyFireDamage(choiceContext, BreakVal);
    }

    private async Task ApplyFireDamage(PlayerChoiceContext choiceContext, decimal amount)
    {
        var list = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsHittable).ToList();
        if (list.Count == 0)
        {
            return;
        }

        var target = Owner.RunState.Rng.CombatTargets.NextItem(list);
        if (target != null)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(target));
            PlayPassiveSfx();
            await CreatureCmd.Damage(choiceContext, target, amount, ValueProp.Unpowered, Owner.Creature);
        }
    }
}