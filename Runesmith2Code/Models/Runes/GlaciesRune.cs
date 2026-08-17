#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Uncommon;
using Runesmith2.Runesmith2Code.Powers;

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

    public override (decimal, decimal) BottomValue => (CalculatedPassiveVal, CalculatedBreakVal);

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.EndTurn;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Glacies>();

    public override async Task<bool> BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
        return true;
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        Trigger();
        PlayPassiveSfx();
        await ApplyIceCold(choiceContext, CalculatedPassiveVal);
        UseCharge();
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await ApplyIceCold(choiceContext, CalculatedBreakVal);
    }

    private async Task ApplyIceCold(PlayerChoiceContext choiceContext, decimal amount)
    {
        var targets = GetHittableCreatures();
        if (targets.Count == 0) return;

        await PowerCmd.Apply<IceColdPower>(choiceContext, targets, amount, Owner.Creature, null);
    }
}