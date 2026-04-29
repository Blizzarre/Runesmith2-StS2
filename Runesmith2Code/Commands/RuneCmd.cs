#region

using MegaCrit.Sts2.Core.Combat;
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
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            var combatState = player.Creature.CombatState!;
            var runeQueue = player.PlayerCombatState?.RuneQueue();
            if (runeQueue == null) return; // todo log warning/error?
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
            modifiedPotency = RunesmithHook.ModifyPotency(combatState, player, modifiedPotency, ValueProp.Move,
                cardPlay?.Card, cardPlay, out var potencyModifiers);
            await RunesmithHook.AfterModifyingPotency(combatState, potencyModifiers);
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

    public static async Task AddPotency(PlayerChoiceContext choiceContext, IEnumerable<RuneModel> runes, Player player,
        CardPlay? cardPlay, decimal potency = 0, ValueProp props = ValueProp.Move)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            var combatState = player.Creature.CombatState!;
            var runeQueue = player.PlayerCombatState?.RuneQueue();
            if (runeQueue == null) return; // todo log warning/error?

            var modifiedPotency = potency;
            modifiedPotency = RunesmithHook.ModifyPotency(combatState, player, modifiedPotency, props,
                cardPlay?.Card, cardPlay, out var potencyModifiers);
            await RunesmithHook.AfterModifyingPotency(combatState, potencyModifiers);

            foreach (var rune in runes) rune.PassiveVal = (int)Math.Max(0, rune.PassiveVal + modifiedPotency);
        }
    }

    public static void RemovePotency(IEnumerable<RuneModel> runes, Player player, decimal potency = 0)
    {
        if (CombatManager.Instance.IsOverOrEnding) return;
        var runeQueue = player.PlayerCombatState?.RuneQueue();
        if (runeQueue == null) return; // todo log warning/error?

        foreach (var rune in runes) rune.PassiveVal = (int)Math.Max(0, rune.PassiveVal - potency);
    }

    public static RuneModel? ChargeOldest(PlayerChoiceContext choiceContext, Player player,
        int chargeAmount)
    {
        if (CombatManager.Instance.IsOverOrEnding) return null;
        var runeQueue = player.PlayerCombatState?.RuneQueue();
        if (runeQueue == null || runeQueue.Runes.Count <= 0) return null;
        var oldestRune = runeQueue.Runes[0];
        oldestRune.ModifyCharge(chargeAmount);
        return oldestRune;
    }

    public static RuneModel? ChargeNewest(PlayerChoiceContext choiceContext, Player player,
        int chargeAmount)
    {
        if (CombatManager.Instance.IsOverOrEnding) return null;
        var runeQueue = player.PlayerCombatState?.RuneQueue();
        if (runeQueue == null || runeQueue.Runes.Count <= 0) return null;
        var oldestRune = runeQueue.Runes[^1];
        oldestRune.ModifyCharge(chargeAmount);
        return oldestRune;
    }

    public static void ChargeAll(PlayerChoiceContext choiceContext, Player player, int chargeAmount)
    {
        var runeQueue = player.PlayerCombatState?.RuneQueue();
        if (runeQueue == null || runeQueue.Runes.Count <= 0) return;
        Charge(choiceContext, runeQueue.Runes, chargeAmount);
    }

    public static void Charge(PlayerChoiceContext choiceContext, IEnumerable<RuneModel> runes, int chargeAmount)
    {
        foreach (var rune in runes)
            if (!CombatManager.Instance.IsOverOrEnding)
                rune.ModifyCharge(chargeAmount);
    }

    public static void Charge(PlayerChoiceContext choiceContext, RuneModel rune, int chargeAmount)
    {
        if (!CombatManager.Instance.IsOverOrEnding) rune.ModifyCharge(chargeAmount);
    }

    public static void SetCharge(PlayerChoiceContext choiceContext, RuneModel rune, int chargeAmount)
    {
        if (!CombatManager.Instance.IsOverOrEnding) rune.SetCharge(chargeAmount);
    }

    public static async Task Passive(PlayerChoiceContext choiceContext, RuneModel? rune)
    {
        if (!CombatManager.Instance.IsOverOrEnding && rune != null)
        {
            choiceContext.PushModel(rune);
            await rune.Passive(choiceContext);
            choiceContext.PopModel(rune);
        }
    }

    public static async Task<RuneModel?> BreakOldest(PlayerChoiceContext choiceContext, Player player,
        bool dequeue = true)
    {
        var runeQueue = player.PlayerCombatState?.RuneQueue();
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
        var runeQueue = player.PlayerCombatState?.RuneQueue();
        if (runeQueue == null) return;
        if (!runeQueue.HasAny()) return;
        var removed = false;
        if (dequeue)
        {
            removed = runeQueue.Remove(brokenRune);
            NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.RuneManager()?.BreakRuneAnim(brokenRune);
        }

        choiceContext.PushModel(brokenRune);
        await brokenRune.Break(choiceContext);
        choiceContext.PopModel(brokenRune);
        if (player.Creature.CombatState != null)
        {
            // TODO consider hook for after rune broken
            await RunesmithHook.AfterRuneBroken(player.Creature.CombatState, choiceContext, player, brokenRune);
            if (removed)
                brokenRune.RemoveInternal();
        }
    }
}