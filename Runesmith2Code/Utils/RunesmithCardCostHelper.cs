using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers.Models;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Models;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Utils;

public static class RunesmithCardCostHelper
{
    public static CardCostColor GetElementsCostColor(Runesmith2Card card, CombatState? state)
    {
        if (state == null)
        {
            return CardCostColor.Unmodified;
        }

        if (!card.CanPlay(out UnplayableReason reason, out AbstractModel? model)
            && reason.HasFlag(UnplayableReason.StarCostTooHigh)
            && model == card)
        {
            return CardCostColor.InsufficientResources;
        }

        if (TryModifyElementsCostWithHook(card, state, out var hookModifiedCost))
        {
            return CardCostHelper.GetColorForHookModifiedCost(hookModifiedCost.Total, card.BaseElementsCost.Total);
        }

        if (card.TemporaryElementsCost != null)
        {
            return CardCostHelper.GetColorForLocalCost(card.TemporaryElementsCost.Cost, card.BaseElementsCost.Total);
        }

        return CardCostColor.Unmodified;
    }

    private static bool TryModifyElementsCostWithHook(Runesmith2Card card, CombatState state,
        out Elements hookModifiedCost)
    {
        hookModifiedCost = card.BaseElementsCost;
        var isModified = false;
        foreach (var item in state.IterateHookListeners())
        {
            if (item is IRunesmithModel runesmithModel)
                isModified |= runesmithModel.TryModifyElementsCost(card, hookModifiedCost, out hookModifiedCost);
        }

        return isModified;
    }
}