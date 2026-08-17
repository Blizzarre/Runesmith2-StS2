#region

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Character;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class SecretRecipe : Runesmith2Card
{
    public SecretRecipe() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCards(2, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var recipeCards = ModelDb.CardPool<Runesmith2CardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Tags.Contains(RunesmithTags.Recipe))
            .ToList();

        var cards = CardFactory.GetDistinctForCombat(Owner,
                recipeCards, DynamicVars.Cards.IntValue,
                Owner.RunState.Rng.CombatCardGeneration
            )
            .ToList();

        CardModel? card;
        if (DynamicVars.Cards.IntValue > 3)
        {
            // Compat for infinite upgrades
            var prefs = new CardSelectorPrefs(RunesmithCardSelectorPrefs.ChooseCardSelectionPrompt, 0, 1)
            {
                Cancelable = true,
                UnpoweredPreviews = true
            };

            card = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards.OrderBy(c => c.Rarity)
                .ThenBy(c => c.Id)
                .ToList(), Owner, prefs)).FirstOrDefault();
        }
        else
        {
            card = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner, true);
        }

        if (card != null)
        {
            card.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }
}