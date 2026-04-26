#region

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
        description.Add("IfFull", IsRuneSlotsFull());
    }

    protected override bool ShouldGlowRedInternal => IsRuneSlotsFull() && CanPlay();

    protected Runesmith2RecipeCard(int cost, CardType type, CardRarity rarity, TargetType target) : base(cost, type,
        rarity, target)
    {
        WithKeyword(RunesmithKeyword.Recipe);
    }

    // All recipe cards will draw 1 card after being drawn
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature == Owner.Creature && this == card) await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
}