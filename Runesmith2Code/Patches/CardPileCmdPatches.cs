using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Runesmith2.Runesmith2Code.Cards.Uncommon;

namespace Runesmith2.Runesmith2Code.Patches;

// TEMP: Fix for LightningHammer not showing in the receiving player's hand. Should remove this if this is fixed in base game.
[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.GiveToAnotherPlayer), MethodType.Async)]
internal class CardPileCmdGiveToAnotherPlayerPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        FieldInfo? pileTypeField = null;
        FieldInfo? cardField = null;
        
        return new InstructionPatcher(instructions)
            .Match(new InstructionMatcher()
                .ldfld(null).PredicateMatch(o =>
                {
                    if (o is not FieldInfo field) return false;
                    if (!field.Name.Equals("pileType")) return false;
                    pileTypeField = field; // capture FieldInfo of "pileType"
                    return true;
                })
                .br_s()
                .call(AccessTools.PropertyGetter(typeof(CardPile), nameof(CardPile.Type)))
                .ldc_i4_1()
                .ldarg_0()
                .ldfld(null).PredicateMatch(o =>
                {
                    if (o is not FieldInfo field) return false;
                    if (!field.Name.Equals("card")) return false;
                    cardField = field; // capture FieldInfo of "card"
                    return true;
                })
                .callvirt(AccessTools.PropertyGetter(typeof(CardModel), nameof(CardModel.Owner)))
                .callvirt(AccessTools.PropertyGetter(typeof(Player), nameof(Player.Character)))
                .callvirt(AccessTools.PropertyGetter(typeof(CharacterModel), nameof(CharacterModel.TrailPath)))
                .call_any()
                .stloc_s()
            ).Insert([
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, pileTypeField),
                CodeInstruction.LoadArgument(0),
                new CodeInstruction(OpCodes.Ldfld, cardField),
                CodeInstruction.Call(typeof(CardPileCmdGiveToAnotherPlayerPatch), nameof(AddCardToHand))
            ]);
    }

    private static void AddCardToHand(PileType pileType, CardModel card)
    {
        if (card is not LightningHammer) return;
        var targetPileType = card.Pile?.Type ?? pileType;
        if (targetPileType != PileType.Hand) return;
        var newCardNode = CardPileCmd.CreateCardNodeAndUpdateVisuals(card, targetPileType, true);
        var handNode = NCombatRoom.Instance?.Ui.Hand;
        handNode?.Add(newCardNode);
    }
}