#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Models;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Commands;

public static class RuneCmd
{
    public static async Task Craft<T>(PlayerChoiceContext choiceContext, Player player, CardPlay? cardPlay,
        CardModel card, bool upgraded = false) where T : RuneModel
    {
        var charge = card.DynamicVars.TryGetValue(ChargeVar.defaultName, out var var1) ? var1.IntValue : 0;
        var potency = card.DynamicVars.TryGetValue(PotencyVar.defaultName, out var var2) ? var2.IntValue : 0;
        await Craft(choiceContext, ModelDb.Get<T>().ToMutable(), player, cardPlay, charge, potency, upgraded);
    }

    public static async Task Craft<T>(PlayerChoiceContext choiceContext, Player player, CardPlay? cardPlay,
        decimal charge, decimal potency = 0, bool upgraded = false) where T : RuneModel
    {
        await Craft(choiceContext, ModelDb.Get<T>().ToMutable(), player, cardPlay, charge, potency, upgraded);
    }

    public static async Task Craft(PlayerChoiceContext choiceContext, RuneModel rune, Player player, CardPlay? cardPlay,
        decimal charge, decimal potency = 0, bool upgraded = false)
    {
        if (!CombatManager.Instance.IsOverOrEnding && player.Creature.CombatState != null)
        {
            var combatState = player.Creature.CombatState;
            var runeQueue = player.PlayerCombatState?.GetRuneQueue();
            if (runeQueue == null) return;
            rune.AssertMutable();

            if (runeQueue.IsFull())
            {
                var playerDialogueLine = new LocString("combat_messages", "RUNESMITH2-FULL_RUNE_SLOTS");
                player.Creature.GetVfxContainer()
                    ?.AddChildSafely(NThoughtBubbleVfx.Create(playerDialogueLine.GetFormattedText(), player.Creature,
                        1.0));
                return;
            }

            var modifiedPotency = potency;
            if (rune.UsePotency)
            {
                modifiedPotency = RunesmithHook.ModifyPotency(combatState, player, modifiedPotency, ValueProp.Move,
                    cardPlay?.Card, cardPlay, out var potencyModifiers);
                await RunesmithHook.AfterModifyingPotency(potencyModifiers);
            }

            var modifiedCharge = charge;
            modifiedCharge = RunesmithHook.ModifyCharge(combatState, player, modifiedCharge, ValueProp.Move,
                cardPlay?.Card, cardPlay, out var chargeModifiers);
            await RunesmithHook.AfterModifyingCharge(combatState, chargeModifiers);

            rune.ChargeVal = (int)Math.Max(0, modifiedCharge);
            rune.PassiveVal = (int)Math.Max(0, modifiedPotency);
            if (upgraded)
                rune.Upgrade();
            rune.Owner = player;
            if (await runeQueue.TryEnqueue(rune))
            {
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

    // Only use this to directly add Rune and bypass Craft Hooks
    public static async Task AddRune(PlayerChoiceContext choiceContext, RuneModel rune, Player player,
        CardPlay? cardPlay)
    {
        var runeQueue = player.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null) return;

        if (runeQueue.IsFull())
        {
            var playerDialogueLine = new LocString("combat_messages", "RUNESMITH2-FULL_RUNE_SLOTS");
            player.Creature.GetVfxContainer()
                ?.AddChildSafely(NThoughtBubbleVfx.Create(playerDialogueLine.GetFormattedText(), player.Creature,
                    1.0));
            return;
        }

        if (await runeQueue.TryEnqueue(rune))
        {
            rune.PlayCraftedSfx();
            var nCreature = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (nCreature != null)
            {
                var runeManager = RunesmithNode.NRuneManager[nCreature];
                runeManager?.AddRuneAnim();
            }
        }
    }

    public static async Task AddPotency(PlayerChoiceContext choiceContext, IEnumerable<RuneModel> runes, Player player,
        CardPlay? cardPlay, decimal potency = 0, ValueProp props = ValueProp.Move)
    {
        if (!CombatManager.Instance.IsOverOrEnding && player.Creature.CombatState != null)
        {
            var combatState = player.Creature.CombatState;
            var runeQueue = player.PlayerCombatState?.GetRuneQueue();
            if (runeQueue == null) return;

            var modifiedPotency = potency;
            modifiedPotency = RunesmithHook.ModifyPotency(combatState, player, modifiedPotency, props,
                cardPlay?.Card, cardPlay, out var potencyModifiers);
            await RunesmithHook.AfterModifyingPotency(potencyModifiers);

            foreach (var rune in runes)
                if (rune.UsePotency)
                    rune.PassiveVal = (int)Math.Max(0, rune.PassiveVal + modifiedPotency);
        }
    }

    public static void RemovePotency(IEnumerable<RuneModel> runes, Player player, decimal potency = 0)
    {
        if (CombatManager.Instance.IsOverOrEnding) return;
        var runeQueue = player.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null) return;

        foreach (var rune in runes) rune.PassiveVal = (int)Math.Max(0, rune.PassiveVal - potency);
    }

    public static RuneModel? ChargeOldest(PlayerChoiceContext choiceContext, Player player,
        int chargeAmount)
    {
        if (CombatManager.Instance.IsOverOrEnding) return null;
        var runeQueue = player.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null || runeQueue.Runes.Count <= 0) return null;
        var oldestRune = runeQueue.Runes[0];
        RunesmithModSounds.PlayRuneChargeSfx();
        oldestRune.ModifyCharge(chargeAmount);
        return oldestRune;
    }

    public static RuneModel? ChargeNewest(PlayerChoiceContext choiceContext, Player player,
        int chargeAmount)
    {
        if (CombatManager.Instance.IsOverOrEnding) return null;
        var runeQueue = player.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null || runeQueue.Runes.Count <= 0) return null;
        var newestRune = runeQueue.Runes[^1];
        RunesmithModSounds.PlayRuneChargeSfx();
        newestRune.ModifyCharge(chargeAmount);
        return newestRune;
    }

    public static void ChargeAll(PlayerChoiceContext choiceContext, Player player, int chargeAmount)
    {
        var runeQueue = player.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null || runeQueue.Runes.Count <= 0) return;
        ChargeRunes(choiceContext, runeQueue.Runes, chargeAmount);
    }

    public static void ChargeRunes(PlayerChoiceContext choiceContext, IEnumerable<RuneModel> runes, int chargeAmount)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            var runesList = runes.ToList();
            if (chargeAmount > 0 && runesList.Count != 0) RunesmithModSounds.PlayRuneChargeSfx();

            foreach (var rune in runesList) rune.ModifyCharge(chargeAmount);
        }
    }

    public static void Charge(PlayerChoiceContext choiceContext, RuneModel rune, int chargeAmount)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            RunesmithModSounds.PlayRuneChargeSfx();
            rune.ModifyCharge(chargeAmount);
        }
    }

    public static void SetCharge(PlayerChoiceContext choiceContext, IEnumerable<RuneModel> runes, int chargeAmount)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            var runesList = runes.ToList();
            if (chargeAmount > 0 && runesList.Count != 0) RunesmithModSounds.PlayRuneChargeSfx();

            foreach (var rune in runesList) rune.SetCharge(chargeAmount);
        }
    }

    public static async Task Passive(PlayerChoiceContext choiceContext, Player player, RuneModel? rune, int count,
        bool isPowered = true)
    {
        if (rune == null) return;
        await Passive(choiceContext, player, [rune], count, isPowered);
    }

    public static async Task Passive(PlayerChoiceContext choiceContext, Player player, IEnumerable<RuneModel> runes,
        int count, bool isPowered = true)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            if (isPowered)
            {
                count = RunesmithHook.ModifyRunePassiveTriggerCount(player.Creature.CombatState!, player, count,
                    out var modifyingModels);
                await RunesmithHook.AfterModifyingRunePassiveTriggerCount(modifyingModels);
            }

            var runesList = runes.ToList();
            for (var i = 0; i < count; i++)
                foreach (var rune in runesList.Where(rune => rune.CanPassive))
                {
                    choiceContext.PushModel(rune);
                    await rune.Passive(choiceContext);
                    choiceContext.PopModel(rune);
                    await Cmd.CustomScaledWait(0.1f, 0.2f);
                }
        }
    }

    public static async Task PassiveAll(PlayerChoiceContext choiceContext, Player player, int count)
    {
        var runeQueue = player.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null || runeQueue.Runes.Count <= 0) return;

        await Passive(choiceContext, player, runeQueue.Runes, count);
    }

    public static async Task<RuneModel?> BreakOldest(PlayerChoiceContext choiceContext, Player player,
        bool dequeue = true)
    {
        var runeQueue = player.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null || !runeQueue.HasAny()) return null;
        var rune = runeQueue.Runes[0];
        choiceContext.PushModel(rune);
        await Break(choiceContext, player, rune, dequeue);
        choiceContext.PopModel(rune);
        return rune;
    }

    public static async Task Break(PlayerChoiceContext choiceContext, Player player, RuneModel? brokenRune,
        bool dequeue = true)
    {
        if (CombatManager.Instance.IsOverOrEnding || brokenRune == null) return;
        var runeQueue = player.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null) return;
        if (!runeQueue.HasAny()) return;
        var removed = false;
        if (dequeue)
        {
            removed = runeQueue.Remove(brokenRune);
            RunesmithModSounds.PlayRuneBreakSfx();
            NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.RuneManager()?.BreakRuneAnim(brokenRune);
        }
        else
        {
            brokenRune.Trigger();
        }

        choiceContext.PushModel(brokenRune);
        await brokenRune.Break(choiceContext);
        choiceContext.PopModel(brokenRune);
        if (player.Creature.CombatState != null)
        {
            await RunesmithHook.AfterRuneBroken(player.Creature.CombatState, choiceContext, player, brokenRune);
            if (removed)
                brokenRune.RemoveInternal();
        }
    }
}