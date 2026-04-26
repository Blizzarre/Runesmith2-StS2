#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using Runesmith2.Runesmith2Code.Entities.Runes;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Utils;

public static class RunesmithSubscriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(MainFile.ModId, CollectRuneModels);
    }

    private static IEnumerable<RuneModel> CollectRuneModels(CombatState combatState)
    {
        return combatState.Players
            .Select(p => p.PlayerCombatState?.RuneQueue())
            .OfType<RuneQueue>()
            .SelectMany(rq => rq.Runes)
            .Where(r => r is { HasBeenRemovedFromState: false, Owner.IsActiveForHooks: true });
    }
}