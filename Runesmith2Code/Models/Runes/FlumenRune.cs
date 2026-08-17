#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Uncommon;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Draw card
public class FlumenRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 0;
    public override int ChargeVal { get; set; } = 3;

    public override (bool, bool) ShowBottomLabel => (false, true);

    public override (decimal, decimal) BottomValue => (1, 2);

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.StartTurn;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Flumen>();

    private int CardToDraw { get; set; }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        var ret = player != Owner || CardToDraw == 0 ? count : count + CardToDraw;
        CardToDraw = 0;
        return ret;
    }

    public override Task<bool> SetupTurnStartRuneTrigger(PlayerChoiceContext choiceContext)
    {
        Trigger();
        PlayPassiveSfx();
        CardToDraw += 1;
        UseCharge();
        return Task.FromResult(true);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        Trigger();
        PlayPassiveSfx();
        await DrawCard(choiceContext, 1);
        UseCharge();
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await DrawCard(choiceContext, 2);
    }

    private async Task DrawCard(PlayerChoiceContext choiceContext, decimal amount)
    {
        await CardPileCmd.Draw(choiceContext, amount, Owner);
    }
}