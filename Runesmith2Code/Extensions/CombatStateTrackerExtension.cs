using MegaCrit.Sts2.Core.Combat;
using Runesmith2.Runesmith2Code.Structs;
using static Runesmith2.Runesmith2Code.Extensions.PlayerCombatStateExtension;

namespace Runesmith2.Runesmith2Code.Extensions;

public static class CombatStateTrackerExtension
{
    private static void OnElementsChanged(this CombatStateTracker tracker, Elements _, Elements __)
    {
        tracker.NotifyCombatStateChanged("OnPlayerCombatStateValueChanged");
    }

    public static void SubscribeElements(this CombatStateTracker tracker, RunesmithCombatState combatState)
    {
        combatState.ElementsChanged += tracker.OnElementsChanged;
    }
    
    public static void UnsubscribeElements(this CombatStateTracker tracker, RunesmithCombatState combatState)
    {
        combatState.ElementsChanged -= tracker.OnElementsChanged;
    }
}