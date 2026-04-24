using Runesmith2.Runesmith2Code.Models;

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyRunePassiveTriggerCount
{
    public int ModifyRunePassiveTriggerCounts(RuneModel rune, int triggerCount);
}

public interface IAfterModifyingRunePassiveTriggerCount
{
    public Task AfterModifyingRunePassiveTriggerCount(RuneModel rune);
}