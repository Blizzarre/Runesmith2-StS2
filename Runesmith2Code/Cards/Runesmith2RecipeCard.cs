#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards;

public abstract class Runesmith2RecipeCard : Runesmith2Card
{
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("IfFull", IsRuneSlotsFull());
    }

    protected override bool ShouldGlowRedInternal => IsRuneSlotsFull() && CanPlay();

    protected Runesmith2RecipeCard(int cost, CardType type, CardRarity rarity, TargetType target) : base(cost, type,
        rarity, target)
    {
        WithKeyword(RunesmithKeyword.Recipe);
    }

    // All recipe cards will draw 1 card after being drawn (only once a turn)
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner.Creature || this != card)
            return;

        // Do not draw more card if another Recipe card was already drawn this turn.
        if (CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>().Any(ce =>
                ce.HappenedThisTurn(card.CombatState) && ce.Card.Owner == Owner && ce.Card != card &&
                ce.Card.Keywords.Contains(RunesmithKeyword.Recipe)))
            return;
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
}