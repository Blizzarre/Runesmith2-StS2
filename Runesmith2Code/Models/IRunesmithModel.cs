using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Models;

public interface IRunesmithModel
{
    public int ModifyEnhanceAmount(Player player, int amount)
    {
        return amount;
    }

    public Task AfterModifyingEnhanceAmount(int modifiedEnhance, CardModel? cardSource, CardPlay? cardPlay)
    {
        return Task.CompletedTask;
    }
    
    public Task AfterCardEnhanced(PlayerChoiceContext choiceContext, CardModel card, int enhanceAmount)
    {
        return Task.CompletedTask;
    }
    
    public int ModifyRunePassiveTriggerCounts(RuneModel rune, int triggerCount)
    {
        return triggerCount;
    }

    public Task AfterModifyingRunePassiveTriggerCount(RuneModel rune)
    {
        return Task.CompletedTask;
    }

    public decimal ModifyRuneValue(Player player, decimal value)
    {
        return value;
    }
    
    public Task AfterElementsGained(Elements elements, Player spender)
    {
        return Task.CompletedTask;
    }
    
    public Task AfterElementsSpent(Elements elements, Player spender)
    {
        return Task.CompletedTask;
    }

    // todo use this in card cost helper?
    public bool TryModifyElementsCost(CardModel card, Elements originalCost, out Elements modifiedCost)
    {
        modifiedCost = originalCost;
        return false;
    }

    public Task AfterRuneCrafted(PlayerChoiceContext choiceContext, Player player, RuneModel rune)
    {
        return Task.CompletedTask;
    }

    public decimal ModifyPotencyAdditive(Player player, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        return 0;
    }
    
    public decimal ModifyPotencyMultiplicative(Player player, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        return 1;
    }

    public Elements ModifyElementsGain(Player player, Elements amount)
    {
        return amount;
    }

    public async Task AfterModifyingElementsGain()
    {
        await Task.CompletedTask;
    }
}