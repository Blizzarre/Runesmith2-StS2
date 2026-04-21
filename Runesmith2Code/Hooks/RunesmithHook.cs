using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Models;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Hooks;

public static class RunesmithHook
{
    public static int ModifyEnhanceAmount(CombatState combatState, Player player, int originalAmount,
        out IEnumerable<AbstractModel> modifiers)
    {
        var modifyingModels = new List<AbstractModel>();
        var modifiedEnhance = originalAmount;
        
        foreach (AbstractModel model in combatState.IterateHookListeners())
        {
            var runesmithModel = model as IRunesmithModel;
            if (runesmithModel == null) continue;
            var addEnhance = runesmithModel.ModifyEnhanceAmount(player, modifiedEnhance);
            modifiedEnhance += addEnhance;
            if (addEnhance != 0)
            {
                modifyingModels.Add(model);
            }
        }
        modifiers = modifyingModels;
        return modifiedEnhance;
    }

    public static async Task AfterModifyingEnhanceAmount(CombatState combatState, int modifiedEnhance, CardModel? cardSource, CardPlay? cardPlay, IEnumerable<AbstractModel> modifiers)
    {
        var abstractModels = modifiers.ToHashSet();
        foreach (var modifier in combatState.IterateHookListeners())
        {
            if (!abstractModels.Contains(modifier)) continue;
            if (modifier is not IRunesmithModel runesmithModel) continue;
            await runesmithModel.AfterModifyingEnhanceAmount(modifiedEnhance, cardSource, cardPlay);
            modifier.InvokeExecutionFinished();
        }
    }
    
    public static async Task AfterCardEnhanced(CombatState combatState, PlayerChoiceContext choiceContext, CardModel card, int enhanceAmount)
    {
        foreach (var model in combatState.IterateHookListeners())
        {
            if (model is not IRunesmithModel runesmithModel) continue;
            choiceContext.PushModel(model);
            await runesmithModel.AfterCardEnhanced(choiceContext, card, enhanceAmount);
            model.InvokeExecutionFinished();
            choiceContext.PopModel(model);
        }
    }

    public static int ModifyingRunePassiveTriggerCount(CombatState combatState, RuneModel rune, int triggerCount,
        out List<AbstractModel> modifyingModels)
    {
        modifyingModels = [];
        var currCount = triggerCount;
        foreach (var model in combatState.IterateHookListeners())
        {
            var prevCount = currCount;
            if (model is not IRunesmithModel runesmithModel) continue;
            currCount = runesmithModel.ModifyRunePassiveTriggerCounts(rune, currCount);
            if (prevCount != currCount)
            {
                modifyingModels.Add(model);
            }
        }
        return currCount;
    }

    public static async Task AfterModifyingRunePassiveTriggerCount(CombatState combatState, RuneModel rune,
        IEnumerable<AbstractModel> modifiers)
    {
        var abstractModels = modifiers.ToHashSet();
        foreach (var modifier in combatState.IterateHookListeners())
        {
            if (!abstractModels.Contains(modifier)) continue;
            if (modifier is not IRunesmithModel runesmithModel) continue;
            await runesmithModel.AfterModifyingRunePassiveTriggerCount(rune);
            modifier.InvokeExecutionFinished();
        }
    }
    
    public static decimal ModifyRuneValue(CombatState combatState, Player player, decimal amount)
    {
        var num = amount;
        foreach (var model in combatState.IterateHookListeners())
        {
            if(model is not IRunesmithModel runesmithModel) continue;
            num = runesmithModel.ModifyRuneValue(player, num);
        }
        return num;
    }

    public static Elements ModifyElementsGain(CombatState combatState, Player player, Elements originalAmount,
        out IEnumerable<AbstractModel> modifiers)
    {
        var modifiedAmount = originalAmount;
        var modifyingModels = new List<AbstractModel>();
        foreach (var model in combatState.IterateHookListeners())
        {
            if(model is not IRunesmithModel runesmithModel) continue;
            modifiedAmount = runesmithModel.ModifyElementsGain(player, modifiedAmount);
        }

        modifiers = modifyingModels;
        return modifiedAmount;
    }

    public static Elements ModifyElementsCost(CombatState combatState, CardModel card, Elements originalCost)
    {
        if (originalCost.Total < 0)
        {
            return originalCost;
        }

        var modifiedCost = originalCost;
        foreach (var model in combatState.IterateHookListeners())
        {
            var runesmithModel = model as IRunesmithModel;
            runesmithModel?.TryModifyElementsCost(card, modifiedCost, out modifiedCost);
        }
        return modifiedCost;
    }

    public static async Task AfterElementsSpent(CombatState combatState, Elements amount, Player spender)
    {
        foreach (var model in combatState.IterateHookListeners())
        {
            var runesmithModel = model as IRunesmithModel;
            if  (runesmithModel != null) await runesmithModel.AfterElementsSpent(amount, spender);
        }
    }

    public static async Task AfterRuneCrafted(CombatState combatState, PlayerChoiceContext choiceContext, Player player,
        RuneModel rune)
    {
        foreach (var model in combatState.IterateHookListeners())
        {
            var runesmithModel = model as IRunesmithModel;
            if (runesmithModel != null)
            {
                choiceContext.PushModel(model);
                await runesmithModel.AfterRuneCrafted(choiceContext, player, rune);
                model.InvokeExecutionFinished();
                choiceContext.PopModel(model);
            }
        }
    }

    public static decimal ModifyPotency(CombatState combatState, Player player, decimal potency, ValueProp props,
        CardModel? cardSource, CardPlay? cardPlay, out IEnumerable<AbstractModel> modifiers)
    {
        var modifyingModels = new List<AbstractModel>();
        var modifiedPotency = potency;
        
        //TODO add enchantment modification here if it's implemented
        
        foreach (AbstractModel model in combatState.IterateHookListeners())
        {
            var runesmithModel = model as IRunesmithModel;
            if (runesmithModel == null) continue;
            var addPotency = runesmithModel.ModifyPotencyAdditive(player, modifiedPotency, props, cardSource, cardPlay);
            modifiedPotency += addPotency;
            if (addPotency != 0)
            {
                modifyingModels.Add(model);
            }
        }
        foreach (AbstractModel model in combatState.IterateHookListeners())
        {
            var runesmithModel = model as IRunesmithModel;
            if (runesmithModel == null) continue;
            var addPotency = runesmithModel.ModifyPotencyMultiplicative(player, modifiedPotency, props, cardSource, cardPlay);
            modifiedPotency *= addPotency;
            if (addPotency != 1)
            {
                modifyingModels.Add(model);
            }
        }
        modifiers = modifyingModels;
        return Math.Max(0, modifiedPotency);
    }

    public static async Task AfterModifyingElementsGain(CombatState combatState, IEnumerable<AbstractModel> modifiers)
    {
        var abstractModels = modifiers.ToHashSet();
        foreach (var modifier in combatState.IterateHookListeners())
        {
            if (!abstractModels.Contains(modifier)) continue;
            if (modifier is not IRunesmithModel runesmithModel) continue;
            await runesmithModel.AfterModifyingElementsGain();
            modifier.InvokeExecutionFinished();
        }
    }
}