#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class OneForEveryone : Runesmith2Card
{
    public OneForEveryone() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(RunesmithHoverTip.Break);
        WithCostUpgradeBy(-1);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override bool ShouldGlowGoldInternal => HasRune();

    public override RuneBreakType RuneBreakType => RuneBreakType.Oldest;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!HasRune()) return;

        var brokenRune = await RuneCmd.BreakOldest(choiceContext, Owner);
        if (CombatState != null && brokenRune != null)
        {
            var teammates = CombatState.GetTeammatesOf(Owner.Creature)
                .Where(c => c is { IsAlive: true, IsPlayer: true } && c.Player != Owner).ToList();

            List<RuneModel> clonedRunes = [];

            foreach (var ally in teammates)
            {
                if (ally.Player == null) continue;
                var clone = brokenRune.CreateClone();
                clone.TransferOwner(ally.Player);
                clonedRunes.Add(clone);
                await RuneCmd.AddRune(choiceContext, clone, ally.Player, play);
            }

            foreach (var rune in clonedRunes)
                if (rune.Owner.PlayerCombatState?.GetRuneQueue()?.Runes.Contains(rune) ?? false)
                    await RuneCmd.Break(choiceContext, rune.Owner, rune);
        }
    }
}