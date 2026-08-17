#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Relics;

public class CrystallizedRainbow : Runesmith2Relic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override bool ShowCounter => true;

    public override int DisplayAmount =>
        IsActivating ? DynamicVars.Energy.IntValue : EnergySpent % DynamicVars.Energy.IntValue;

    [SavedProperty]
    private int EnergySpent
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            UpdateDisplay();
        }
    }

    private bool IsActivating
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            UpdateDisplay();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ElementsVar(1),
        new EnergyVar(5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        RunesmithHoverTipFactory.CreateElementsHoverTip()
    ];

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            var threshold = DynamicVars.Energy.IntValue;
            Status = EnergySpent % threshold == threshold - 1 ? RelicStatus.Active : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner != Owner || !CombatManager.Instance.IsInProgress)
            return;

        var threshold = DynamicVars.Energy.IntValue;
        EnergySpent += amount;
        (var triggers, EnergySpent) = Math.DivRem(EnergySpent, threshold);

        if (triggers <= 0) return;
        _ = TaskHelper.RunSafely(DoActivateVisuals());
        for (var i = 0; i < triggers; i++)
            await RunesmithPlayerCmd.GainElements(new Elements(DynamicVars[ElementsVar.defaultName].IntValue), Owner);
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}