using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Models;

namespace Runesmith2.Runesmith2Code.Commands;

public static class RuneCmd
{
    public static async Task Craft<T>(PlayerChoiceContext choiceContext, Player player, CardPlay? cardPlay,
        decimal charge, decimal potency = 0) where T : RuneModel
    {
        await Craft(choiceContext, ModelDb.Get<T>().ToMutable(), player, cardPlay, charge, potency);
    }

    public static async Task Craft(PlayerChoiceContext choiceContext, RuneModel rune, Player player, CardPlay? cardPlay,
        decimal charge, decimal potency = 0)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            var combatState = player.Creature.CombatState;
            var runesmithCombatState = RunesmithField.RunesmithCombatState[player.PlayerCombatState];
            var runeQueue = runesmithCombatState.RuneQueue;
            rune.AssertMutable();

            // TODO Modify rune charge and potency
            decimal modifiedPotency = potency;
            modifiedPotency = RunesmithHook.ModifyPotency(combatState, player, modifiedPotency, ValueProp.Move,
                cardPlay.Card, cardPlay, out _);
            // TODO after modifying charge/potency

            rune.ChargeVal = (int)Math.Max(0, charge);
            rune.PassiveVal = (int)Math.Max(0, modifiedPotency);
            rune.Owner = player;
            if (await runeQueue.TryEnqueue(rune))
            {
                // TODO add combat history
                rune.PlayCraftedSfx();
                var nCreature = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
                if (nCreature != null)
                {
                    var runeManager = RunesmithNode.NRuneManager[nCreature];
                    runeManager?.AddRuneAnim();
                    await RunesmithHook.AfterRuneCrafted(combatState, choiceContext, player, rune);
                }
            }
        }
    }
}