#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Uncommon;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

public class IncendiumRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 4;
    public override int ChargeVal { get; set; } = 3;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.EndTurn;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Incendium>();

    public override async Task BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            await ApplyAoeFireDamage(choiceContext, PassiveVal);
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await ApplyAoeFireDamage(choiceContext, BreakVal);
    }

    private async Task ApplyAoeFireDamage(PlayerChoiceContext choiceContext, decimal amount)
    {
        var targets = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsHittable).ToList();
        if (targets.Count == 0) return;

        foreach (var target in targets)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(target));
        PlayPassiveSfx();
        await CreatureCmd.Damage(choiceContext, targets, amount, ValueProp.Unpowered, Owner.Creature);
    }
}