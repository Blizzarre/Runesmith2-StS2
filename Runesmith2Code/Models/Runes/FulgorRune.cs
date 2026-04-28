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

// Deal damage, gain Block, draw card
public class FulgorRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 6;
    public override int ChargeVal { get; set; } = 3;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.StartTurn;
    public override (bool, bool) ShowTopLabel => (true, true);
    public override (decimal, decimal) TopValue => (CalculatedPassiveVal, CalculatedBreakVal);
    public override (Color, Color, Color) TopLabelColor => NRune.BlueFontColor;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Fulgor>();

    public override async Task BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            PlayPassiveSfx();
            await ApplyAoeFireDamage(choiceContext, PassiveVal);
            await GainBlock(choiceContext, PassiveVal);
        }
    }

    public override async Task AfterTurnStartRuneTrigger(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            PlayPassiveSfx();
            await DrawCard(choiceContext, 1);
            UseCharge();
        }
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            PlayPassiveSfx();
            await ApplyAoeFireDamage(choiceContext, PassiveVal);
            await GainBlock(choiceContext, PassiveVal);
            await DrawCard(choiceContext, 1);
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        PlayBreakSfx();
        await ApplyAoeFireDamage(choiceContext, BreakVal);
        await GainBlock(choiceContext, BreakVal);
        await DrawCard(choiceContext, 2);
    }

    private async Task ApplyAoeFireDamage(PlayerChoiceContext choiceContext, decimal amount)
    {
        var targets = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsHittable).ToList();
        if (targets.Count == 0) return;

        foreach (var target in targets)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(target));
        await CreatureCmd.Damage(choiceContext, targets, amount, ValueProp.Unpowered, Owner.Creature);
    }

    private async Task GainBlock(PlayerChoiceContext _, decimal amount)
    {
        await CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Unpowered, null);
    }

    private async Task DrawCard(PlayerChoiceContext choiceContext, decimal amount)
    {
        await CardPileCmd.Draw(choiceContext, amount, Owner);
    }
}