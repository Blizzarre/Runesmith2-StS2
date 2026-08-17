#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Relics;

public class BallPeenHammer : Runesmith2Relic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount =>
        !IsActivating ? SkillsPlayedThisTurn % DynamicVars.Cards.IntValue : DynamicVars.Cards.IntValue;

    private int SkillsPlayedThisTurn
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
        new CardsVar(3),
        new EnhanceByVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        RunesmithHoverTipFactory.Static(RunesmithHoverTip.Enhance)
    ];

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            var threshold = DynamicVars.Cards.IntValue;
            Status = SkillsPlayedThisTurn % threshold == threshold - 1 ? RelicStatus.Active : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override Task BeforeCombatStart()
    {
        SkillsPlayedThisTurn = 0;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState is { TurnNumber: 1 })
            return Task.CompletedTask;
        SkillsPlayedThisTurn = 0;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Skill)
            return;
        SkillsPlayedThisTurn++;
        var cards = DynamicVars.Cards.IntValue;
        if (SkillsPlayedThisTurn % cards != 0)
            return;
        _ = TaskHelper.RunSafely(DoActivateVisuals());
        await RunesmithCardCmd.EnhanceRandomCards(choiceContext, Owner, PileType.Hand.GetPile(Owner).Cards,
            1, 1, Owner.RunState.Rng.CombatCardSelection);
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        IsActivating = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}