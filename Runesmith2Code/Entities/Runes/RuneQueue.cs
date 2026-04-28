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

    private readonly Player _owner;

    private readonly List<RuneModel> _runes = new();

    public IReadOnlyList<RuneModel> Runes => _runes;

    public int Capacity => MaxCapacity;

    public RuneQueue(Player owner)
    {
        _owner = owner;
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
        if (_owner.Creature.CombatState == null) return;

        var count = RunesmithHook.ModifyRunePassiveTriggerCount(_owner.Creature.CombatState, _owner,1,
            out var modifyingModels);
        await RunesmithHook.AfterModifyingRunePassiveTriggerCount(_owner.Creature.CombatState,
            modifyingModels);

        var runeTriggerCount = Runes.Select(rune => new Tuple<RuneModel, int>(rune, Math.Min(rune.ChargeVal, count)))
            .ToList();
        foreach (var pair in runeTriggerCount)
            for (var i = 0; i < pair.Item2; i++)
            {
                var wait = await pair.Item1.BeforeTurnEndEarlyRuneTrigger(choiceContext);
                if (wait)
                    await SmallWait();
            }

        // refresh count in case the rune charges has changed due to early trigger
        runeTriggerCount = Runes.Select(rune => new Tuple<RuneModel, int>(rune, Math.Min(rune.ChargeVal, count)))
            .ToList();
        foreach (var pair in runeTriggerCount)
            for (var i = 0; i < pair.Item2; i++)
            {
                await pair.Item1.BeforeTurnEndRuneTrigger(choiceContext);
                await SmallWait();
            }
    }

    public async Task AfterTurnStart(PlayerChoiceContext choiceContext)
    {
        if (_owner.Creature.CombatState == null) return;

        var count = RunesmithHook.ModifyRunePassiveTriggerCount(_owner.Creature.CombatState, _owner, 1,
            out var modifyingModels);
        await RunesmithHook.AfterModifyingRunePassiveTriggerCount(_owner.Creature.CombatState,
            modifyingModels);

        var runeTriggerCount = Runes.Select(rune => new Tuple<RuneModel, int>(rune, Math.Min(rune.ChargeVal, count)))
            .ToList();

        foreach (var pair in runeTriggerCount)
            for (var i = 0; i < pair.Item2; i++)
            {
                await pair.Item1.AfterTurnStartRuneTrigger(choiceContext);
                await SmallWait();
            }
    }

    private async Task SmallWait()
    {
        if (LocalContext.IsMe(_owner))
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        else
            await Cmd.Wait(0.05f);
    }
}