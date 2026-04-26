namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyRunePassiveTriggerCount
{
    public int ModifyRunePassiveTriggerCounts(int triggerCount);
}

public interface IAfterModifyingRunePassiveTriggerCount
{
    public Task AfterModifyingRunePassiveTriggerCount();
}