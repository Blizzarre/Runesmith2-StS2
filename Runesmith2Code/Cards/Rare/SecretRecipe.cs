#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Character;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class SecretRecipe : Runesmith2Card
{
    public SecretRecipe() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cards = CardFactory.GetDistinctForCombat(Owner,
                ModelDb.CardPool<Runesmith2CardPool>()
                    .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                    .Where(c => c.Keywords.Contains(RunesmithKeyword.Recipe)), 3,
                Owner.RunState.Rng.CombatCardGeneration
            )
            .ToList();

        var card = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner, true);

        if (card != null)
        {
            card.SetToFreeThisCombat();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner);
        }
    }
}