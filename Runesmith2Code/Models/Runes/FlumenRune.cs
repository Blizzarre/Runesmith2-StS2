#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Uncommon;
using Runesmith2.Runesmith2Code.Utils;

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

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await Passive(choiceContext);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            Trigger();
            await DrawCard(choiceContext, 1);
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await DrawCard(choiceContext, 2);
    }

    private async Task DrawCard(PlayerChoiceContext choiceContext, decimal amount)
    {
        PlayPassiveSfx();
        await CardPileCmd.Draw(choiceContext, amount, Owner);
    }
}