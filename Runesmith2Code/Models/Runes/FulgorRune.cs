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
using Runesmith2.Runesmith2Code.Cards.Ancient;
using Runesmith2.Runesmith2Code.Nodes.Runes;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Deal damage multiple times
public class FulgorRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 5;
    public override int ChargeVal { get; set; } = 4;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.EndTurn;
    
    
    public override (bool, bool) ShowTopLabel => (true, true);
    public override (decimal, decimal) TopValue => (PassiveVal, PassiveVal);
    public override (Color, Color, Color) TopLabelColor => NRune.DefaultFontColor;
    public override (Color, Color, Color) TopLabelBreakColor => NRune.DefaultFontColor;

    public override (bool, bool) ShowBottomLabel => (true, true);
    public override (decimal, decimal) BottomValue => (-1, -1);
    public override (string, string) BottomTextAppend => ("x2", "x4");
    public override (Color, Color, Color) BottomLabelColor => NRune.BlueFontColor;

    public override decimal BreakVal => PassiveVal;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Fulgor>();

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
            await ApplyFireDamage(choiceContext, PassiveVal, 2);
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        PlayBreakSfx();
        await ApplyFireDamage(choiceContext, PassiveVal, 4);
    }
    
    private async Task ApplyFireDamage(PlayerChoiceContext choiceContext, decimal amount, int count)
    {
        PlayPassiveSfx();
        for (var i = 0; i < count; i++)
        {
            var list = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsHittable).ToList();
            if (list.Count == 0) return;
            
            var target = Owner.RunState.Rng.CombatTargets.NextItem(list);
            if (target == null) break; // Should be okay to break when there are no valid targets
            
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(target));
            await CreatureCmd.Damage(choiceContext, target, amount, ValueProp.Unpowered, Owner.Creature);
        }
    }
}