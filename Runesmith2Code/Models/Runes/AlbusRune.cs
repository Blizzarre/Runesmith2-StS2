#region

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Uncommon;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Nodes.Runes;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Give Charge
public class AlbusRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 0;
    public override int ChargeVal { get; set; } = 2;

    public override (bool, bool) ShowBottomLabel => (false, true);

    public override (decimal, decimal) BottomValue => (1, 2);

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.EndTurn;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Albus>();

    public override bool CanPassive => HasAnyValidRune() && base.CanPassive;

    public override bool UsePotency => true;

    public override async Task<bool> BeforeTurnEndEarlyRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
        return true;
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        Trigger();
        PlayPassiveSfx();
        await ChargeAndAddPotency(choiceContext, 1, 0);
        UseCharge();
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await ChargeAndAddPotency(choiceContext, 2, 0);
    }

    private async Task ChargeAndAddPotency(PlayerChoiceContext choiceContext, int chargeAmount, decimal potencyAmount)
    {
        var runeQueue = Owner.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null) return;

        if (chargeAmount > 0)
            RuneCmd.ChargeRunes(choiceContext, runeQueue.Runes.Where(r => r is not AlbusRune), chargeAmount);

        if (potencyAmount > 0)
            await RuneCmd.AddPotency(choiceContext, runeQueue.Runes.Where(r => r is not AlbusRune), Owner, null,
                potencyAmount, ValueProp.Unpowered);
        await Cmd.CustomScaledWait(0.2f, 0.3f);
    }

    private bool HasAnyValidRune()
    {
        var runeQueue = Owner.PlayerCombatState?.GetRuneQueue();
        return runeQueue != null && runeQueue.Runes.Any(r => r is not AlbusRune);
    }
}