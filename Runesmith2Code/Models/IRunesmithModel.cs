using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Runesmith2.Runesmith2Code.Models;

public interface IRunesmithModel
{
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
}