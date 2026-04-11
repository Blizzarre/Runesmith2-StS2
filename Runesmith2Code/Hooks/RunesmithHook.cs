using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Models;

namespace Runesmith2.Runesmith2Code.Hooks;

public static class RunesmithHook
{
    public static async Task AfterCardEnhanced(CombatState? combatState, PlayerChoiceContext choiceContext, CardModel card, int enhanceAmount)
    {
        if (combatState == null)
            return;
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
        foreach (var item in combatState.IterateHookListeners())
        {
            if(item is not IRunesmithModel runesmithModel) continue;
            num = runesmithModel.ModifyRuneValue(player, num);
        }
        return num;
    }
}