using MegaCrit.Sts2.Core.Combat;
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
            if (!(model is IRunesmithModel)) continue;
            choiceContext.PushModel(model);
            await (model as IRunesmithModel).AfterCardEnhanced(choiceContext, card, enhanceAmount);
            model.InvokeExecutionFinished();
            choiceContext.PopModel(model);
        }
    }
}