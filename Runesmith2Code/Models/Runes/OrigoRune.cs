#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Rare;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Add card to hand
public class OrigoRune : RuneModel
{
    // TODO show upgraded visual
    public override decimal PassiveVal { get; set; } = 0;
    public override int ChargeVal { get; set; } = 3;

    public override (bool, bool) ShowBottomLabel => (false, true);

    public override (decimal, decimal) BottomValue => (1, 2);

    public override bool IsUpgradeable => true;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.StartTurn;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Origo>();

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await Passive(choiceContext);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            await CreateCard(choiceContext, 2);
            UseCharge();
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await CreateCard(choiceContext, 2);
    }

    private async Task CreateCard(PlayerChoiceContext choiceContext, int amount)
    {
        var cardModels = CardFactory.GetForCombat(Owner,
            from c in Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint)
            where true
            select c, amount, Owner.RunState.Rng.CombatCardGeneration);

        PlayPassiveSfx();
        foreach (var card in cardModels)
        {
            if (Upgraded)
                CardCmd.Upgrade(card);
            card.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }
}