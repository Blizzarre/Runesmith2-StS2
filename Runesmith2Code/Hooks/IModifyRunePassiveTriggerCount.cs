#region

using MegaCrit.Sts2.Core.Entities.Players;

#endregion

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyRunePassiveTriggerCount
{
    public int ModifyRunePassiveTriggerCounts(int triggerCount, Player player);
}

public interface IAfterModifyingRunePassiveTriggerCount
{
    public Task AfterModifyingRunePassiveTriggerCount();
}