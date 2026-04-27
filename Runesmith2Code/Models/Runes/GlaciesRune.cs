#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Uncommon;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Apply Ice-Cold
public class GlaciesRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 0;
    public override int ChargeVal { get; set; } = 3;

    public override decimal CalculatedPassiveVal => 2;

    public override decimal CalculatedBreakVal => CalculatedPassiveVal * 2;

    public override (bool, bool) ShowBottomLabel => (false, true);

    public override (decimal, decimal) BottomValue => (2, 4);

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.EndTurn;

    public override Runesmith2RecipeCard? RecipeCard => ModelDb.Get<Glacies>();

    public override async Task BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            await ApplyIceCold(choiceContext, CalculatedPassiveVal);
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await ApplyIceCold(choiceContext, CalculatedBreakVal);
    }

    private async Task ApplyIceCold(PlayerChoiceContext choiceContext, decimal amount)
    {
        var targets = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsHittable).ToList();
        if (targets.Count == 0) return;

        PlayPassiveSfx();
        await PowerCmd.Apply<IceColdPower>(choiceContext, targets, amount, Owner.Creature, null);
    }
}