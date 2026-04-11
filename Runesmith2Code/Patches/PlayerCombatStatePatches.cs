using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using Runesmith2.Runesmith2Code.Entities.Runes;
using Runesmith2.Runesmith2Code.Field;

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor)]
[HarmonyPatch([typeof(Player)])]
class PlayerCombatStateConstructorPatch
{
    [HarmonyPostfix]
    static void Postfix(Player player, PlayerCombatState __instance)
    {
        var runeQueue = new RuneQueue(player);
        runeQueue.Clear();
        RunesmithField.RuneQueue[__instance] = runeQueue;
    }
}