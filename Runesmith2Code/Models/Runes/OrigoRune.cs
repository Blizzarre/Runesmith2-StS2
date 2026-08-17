#region

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Rare;
using Runesmith2.Runesmith2Code.Nodes.Runes;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Add card to hand
public class OrigoRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 0;
    public override int ChargeVal { get; set; } = 3;

    public override (bool, bool) ShowTopLabel => (false, true);

    public override (decimal, decimal) TopValue => (0, 2);

    public override (bool, bool) ShowBottomLabel => Upgraded ? (true, true) : (false, false);

    public override (string, string) BottomTextAppend => Upgraded ? ("+", "+") : ("", "");

    public override (decimal, decimal) BottomValue => (-1, -1);

    public override (Color, Color, Color) BottomLabelColor => NRune.GreenFontColor;

    public override (Color, Color, Color) BottomBreakColor => NRune.GreenFontColor;

    public override bool IsUpgradeable => true;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.StartTurn;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Origo>();

    public override async Task<bool> SetupTurnStartRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
        return true;
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        Trigger();
        PlayPassiveSfx();
        await CreateCard(choiceContext, 1);
        UseCharge();
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await CreateCard(choiceContext, 2);
    }

    private async Task CreateCard(PlayerChoiceContext choiceContext, int amount)
    {
        var cardModels = CardFactory.GetForCombat(Owner,
            Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint),
            amount, Owner.RunState.Rng.CombatCardGeneration);

        foreach (var card in cardModels)
        {
            if (Upgraded)
                CardCmd.Upgrade(card);
            // card.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }
}