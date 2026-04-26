#region

using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Uncommon;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Utils;

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

    public override Runesmith2RecipeCard? RecipeCard => ModelDb.Get<Albus>().MutableClone() as Runesmith2RecipeCard;

    public override async Task<bool> BeforeTurnEndEarlyRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
        return true;
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            await ChargeRunes(choiceContext, 1);
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await ChargeRunes(choiceContext, 2);
    }

    private Task ChargeRunes(PlayerChoiceContext choiceContext, decimal amount)
    {
        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue == null) return Task.CompletedTask;

        PlayPassiveSfx();
        RuneCmd.Charge(choiceContext, runeQueue.Runes.Where(r => r is not AlbusRune), (int)amount);
        return Task.CompletedTask;
    }
}