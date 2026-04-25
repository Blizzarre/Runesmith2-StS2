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
    private static async Task Dispatch<T>(ICombatState combatState, Func<T, Task> action) where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            await action(model);
            abstractModel.InvokeExecutionFinished();
        }
    }

    private static async Task Dispatch<T>(ICombatState combatState, Func<T, Task> action,
        IEnumerable<AbstractModel> filter) where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>().Intersect(filter.OfType<T>()))
        {
            var abstractModel = (AbstractModel)(object)model;
            await action(model);
            abstractModel.InvokeExecutionFinished();
        }
    }


    private static async Task Dispatch<T>(ICombatState combatState, PlayerChoiceContext choiceContext,
        Func<T, Task> action) where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await action(model);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }

    private static TResult Aggregate<T, TResult>(ICombatState combatState, TResult seed,
        Func<T, TResult, TResult> action) where T : class
    {
        return combatState.IterateHookListeners().OfType<T>()
            .Aggregate(seed, (curr, model) => action(model, curr));
    }


    public static int ModifyEnhanceAmount(ICombatState combatState, Player player, int originalAmount,
        out IEnumerable<AbstractModel> modifiers)
    {
        var modifyingModels = new List<AbstractModel>();
        var res = Aggregate<IModifyEnhanceAmount, int>(combatState, originalAmount, (model, current) =>
        {
            var next = model.ModifyEnhanceAmount(player, current);
            if (next != current) modifyingModels.Add((AbstractModel)model);
            return next;
        });
        modifiers = modifyingModels;
        return res;
    }

    public static Task AfterModifyingEnhanceAmount(ICombatState combatState, int modifiedEnhance,
        CardModel? cardSource, CardPlay? cardPlay, IEnumerable<AbstractModel> modifiers)
    {
        return Dispatch<IAfterModifyingEnhanceAmount>(combatState,
            model => model.AfterModifyingEnhanceAmount(modifiedEnhance, cardSource, cardPlay), modifiers);
    }

    public static Task AfterCardEnhanced(ICombatState combatState, PlayerChoiceContext choiceContext,
        CardModel card, int enhanceAmount)
    {
        return Dispatch<IAfterCardEnhanced>(combatState, choiceContext,
            model => model.AfterCardEnhanced(choiceContext, card, enhanceAmount));
    }

    public static int ModifyRunePassiveTriggerCount(ICombatState combatState, RuneModel rune, int originalCount,
        out List<AbstractModel> modifiers)
    {
        var modifyingModels = new List<AbstractModel>();
        var res = Aggregate<IModifyRunePassiveTriggerCount, int>(combatState, originalCount, (model, current) =>
        {
            var next = model.ModifyRunePassiveTriggerCounts(rune, current);
            if (next != current) modifyingModels.Add((AbstractModel)model);
            return next;
        });
        modifiers = modifyingModels;
        return res;
    }

    public static Task AfterModifyingRunePassiveTriggerCount(ICombatState combatState, RuneModel rune,
        IEnumerable<AbstractModel> modifiers)
    {
        return Dispatch<IAfterModifyingRunePassiveTriggerCount>(combatState,
            model => model.AfterModifyingRunePassiveTriggerCount(rune), modifiers);
    }

    public static decimal ModifyRuneValue(ICombatState combatState, Player player, decimal amount)
    {
        return Aggregate<IModifyRuneValue, decimal>(combatState, amount,
            (model, current) => model.ModifyRuneValue(player, current));
    }

    public static Elements ModifyElementsGain(ICombatState combatState, Player player, Elements originalAmount,
        out IEnumerable<AbstractModel> modifiers)
    {
        var modifyingModels = new List<AbstractModel>();
        var res = Aggregate<IModifyElementsGain, Elements>(combatState, originalAmount, (model, current) =>
        {
            var next = model.ModifyElementsGain(player, current);
            if (next != current) modifyingModels.Add((AbstractModel)model);
            return next;
        });
        modifiers = modifyingModels;
        return res;
    }

    public static Task AfterModifyingElementsGain(ICombatState combatState, IEnumerable<AbstractModel> modifiers)
    {
        return Dispatch<IAfterModifyingElementsGain>(combatState,
            model => model.AfterModifyingElementsGain());
    }

    public static Task AfterElementsGained(ICombatState combatState, Elements amount, Player player,
        CardPlay? cardPlay = null)
    {
        return Dispatch<IAfterElementsGained>(combatState,
            model => model.AfterElementsGained(combatState, amount, player, cardPlay));
    }

    public static Elements ModifyElementsCost(ICombatState combatState, CardModel card, Elements originalCost)
    {
        if (originalCost.Total < 0) return originalCost;

        var modifiedCost = originalCost;
        foreach (var model in combatState.IterateHookListeners().OfType<IModifyElementsCost>())
            model.TryModifyElementsCost(card, modifiedCost, out modifiedCost);

        return modifiedCost;
    }

    public static Task AfterElementsSpent(ICombatState combatState, Elements amount, Player spender)
    {
        return Dispatch<IAfterElementsSpent>(combatState, model => model.AfterElementsSpent(amount, spender));
    }

    public static Task AfterRuneCrafted(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        RuneModel rune)
    {
        return Dispatch<IAfterRuneCrafted>(combatState, choiceContext,
            model => model.AfterRuneCrafted(choiceContext, player, rune));
    }

    public static decimal ModifyPotency(ICombatState combatState, Player player, decimal potency, ValueProp props,
        CardModel? cardSource, CardPlay? cardPlay, out IEnumerable<AbstractModel> modifiers)
    {
        var modifyingModels = new List<AbstractModel>();
        var modifiedPotency = potency;

        //TODO add enchantment modification here if it's implemented

        modifiedPotency = Aggregate<IOnModifyPotencyAdditive, decimal>(combatState, modifiedPotency, (model, current) =>
        {
            var add = model.ModifyPotencyAdditive(player, current, props, cardSource, cardPlay);
            if (add != 0) modifyingModels.Add((AbstractModel)model);
            return add + current;
        });

        modifiedPotency = Aggregate<IOnModifyPotencyMultiplicative, decimal>(combatState, modifiedPotency,
            (model, current) =>
            {
                var mult = model.ModifyPotencyMultiplicative(player, current, props, cardSource, cardPlay);
                if (mult != 1) modifyingModels.Add((AbstractModel)model);
                return mult * current;
            });

        modifiers = modifyingModels;
        return Math.Max(0, modifiedPotency);
    }
}