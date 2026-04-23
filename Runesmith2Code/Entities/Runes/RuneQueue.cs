using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Models;

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

    public async Task<bool> TryEnqueue(RuneModel rune)
    {
        if (Capacity == 0) return false;
        rune.AssertMutable();
        if (Runes.Count >= Capacity)
        {
            throw new InvalidOperationException("RuneQueue is full");
        }

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
        if (idx > Capacity)
        {
            throw new InvalidOperationException("idx cannot be greater than capacity");
        }

        _runes.Insert(idx, rune);
    }

    public async Task BeforeTurnEnd(PlayerChoiceContext choiceContext)
    {
        foreach (var rune in Runes.ToList())
        {
            if (_owner.Creature.CombatState == null) return;

            var triggerCount =
                RunesmithHook.ModifyingRunePassiveTriggerCount(_owner.Creature.CombatState, rune, 1,
                    out var modifyingModels);
            await RunesmithHook.AfterModifyingRunePassiveTriggerCount(_owner.Creature.CombatState, rune,
                modifyingModels);
            if (_owner.Creature.CombatState == null) return;
            for (var i = 0; i < triggerCount; i++)
            {
                await rune.BeforeTurnEndRuneTrigger(choiceContext);
                await SmallWait();
            }
        }
    }

    public async Task AfterTurnStart(PlayerChoiceContext choiceContext)
    {
        foreach (var rune in Runes.ToList())
        {
            if (_owner.Creature.CombatState == null) return;

            var triggerCount =
                RunesmithHook.ModifyingRunePassiveTriggerCount(_owner.Creature.CombatState, rune, 1,
                    out var modifyingModels);
            await RunesmithHook.AfterModifyingRunePassiveTriggerCount(_owner.Creature.CombatState, rune,
                modifyingModels);
            if (_owner.Creature.CombatState == null) return;
            for (var i = 0; i < triggerCount; i++)
            {
                await rune.AfterTurnStartRuneTrigger(choiceContext);
                await SmallWait();
            }
        }
    }

    private async Task SmallWait()
    {
        if (LocalContext.IsMe(_owner))
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
        else
        {
            await Cmd.Wait(0.05f);
        }
    }
}