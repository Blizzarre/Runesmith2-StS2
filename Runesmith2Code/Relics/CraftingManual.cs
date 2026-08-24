#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Structs;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Relics;

public class CraftingManual : Runesmith2Relic, IModifyElementsCost
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    public override Task BeforeCombatStart()
    {
        RecipeCardPlayedThisCombat = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    private bool RecipeCardPlayedThisCombat { get; set; }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner == Owner && card.Tags.Contains(RunesmithTags.Recipe) && !RecipeCardPlayedThisCombat)
        {
            RecipeCardPlayedThisCombat = true;
            Flash();
            Status = RelicStatus.Normal;
        }

        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!ShouldModifyCost(card))
        {
            return false;
        }

        modifiedCost = 0;
        return true;
    }
    
    
    public bool TryModifyElementsCost(CardModel card, Elements originalCost, out Elements modifiedCost)
    {
        modifiedCost = originalCost;
        if (!ShouldModifyCost(card))
        {
            return false;
        }

        modifiedCost = new Elements(0);
        return true;
    }


    private bool ShouldModifyCost(CardModel card)
    {
        return !RecipeCardPlayedThisCombat && card.Owner == Owner && card.Tags.Contains(RunesmithTags.Recipe);
    }
    
    public override Task AfterCombatEnd(CombatRoom _)
    {
        RecipeCardPlayedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}