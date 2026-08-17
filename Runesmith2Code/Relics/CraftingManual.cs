#region

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Relics;

public class CraftingManual : Runesmith2Relic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is { TurnNumber: > 1 }) return;

        var recipes = PileType.Draw.GetPile(player).Cards.Where(c => c.Tags.Contains(RunesmithTags.Recipe)).ToList();
        if (recipes.Count == 0) return;

        var card = player.RunState.Rng.CombatCardSelection.NextItem(recipes);
        if (card == null)
            return;

        Flash();
        card.SetToFreeThisTurn();
        await CardPileCmd.Add(card, PileType.Hand);
    }
}