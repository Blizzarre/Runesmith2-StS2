#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Entities.Runes;

public class RuneQueue
{
    public const int MaxCapacity = 7;

    private Player Owner { get; }

    private readonly List<RuneModel> _runes = [];

    public IReadOnlyList<RuneModel> Runes => _runes;

    public int Capacity { get; set; } = MaxCapacity;

    public RuneQueue(Player owner)
    {
        Owner = owner;
    }

    public void Clear()
    {
        _runes.Clear();
    }

    public bool HasAny()
    {
        return _runes.Count != 0;
    }

    public async Task<bool> TryEnqueue(RuneModel rune)
    {
        if (Capacity == 0) return false;
        rune.AssertMutable();
        if (Runes.Count >= Capacity) throw new InvalidOperationException("RuneQueue is full");

        _runes.Add(rune);
        await SmallWait();
        return true;
    }

    public bool Remove(RuneModel rune)
    {
        return _runes.Remove(rune);
    }

    public void Insert(int idx, RuneModel rune)
    {
        if (idx > Capacity) throw new InvalidOperationException("idx cannot be greater than capacity");

        _runes.Insert(idx, rune);
    }

    public bool IsFull()
    {
        return Runes.Count >= Capacity;
    }

    public async Task BeforeTurnEnd(PlayerChoiceContext choiceContext)
    {
        if (Owner.Creature.CombatState == null) return;

        var count = RunesmithHook.ModifyRunePassiveTriggerCount(Owner.Creature.CombatState, Owner, 1,
            out var modifyingModels);
        await RunesmithHook.AfterModifyingRunePassiveTriggerCount(modifyingModels);

        for (var i = 0; i < count; i++)
            foreach (var rune in Runes)
            {
                if (!rune.CanPassive) continue;
                if (await rune.BeforeTurnEndEarlyRuneTrigger(choiceContext)) await SmallWait();
            }

        await SmallWait();

        for (var i = 0; i < count; i++)
            foreach (var rune in Runes)
            {
                if (!rune.CanPassive) continue;
                if (await rune.BeforeTurnEndRuneTrigger(choiceContext)) await SmallWait();
            }
    }

    public async Task SetupTurnStart(PlayerChoiceContext choiceContext)
    {
        if (Owner.Creature.CombatState == null) return;

        var count = RunesmithHook.ModifyRunePassiveTriggerCount(Owner.Creature.CombatState, Owner, 1,
            out var modifyingModels);
        await RunesmithHook.AfterModifyingRunePassiveTriggerCount(modifyingModels);

        for (var i = 0; i < count; i++)
            foreach (var rune in Runes)
            {
                if (!rune.CanPassive) continue;
                if (await rune.SetupTurnStartRuneTrigger(choiceContext)) await SmallWait();
            }
    }

    private async Task SmallWait()
    {
        if (LocalContext.IsMe(Owner))
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        else
            await Cmd.Wait(0.05f);
    }
}