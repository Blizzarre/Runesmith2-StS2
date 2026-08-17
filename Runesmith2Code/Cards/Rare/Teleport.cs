#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Teleport : Runesmith2Card
{
    public Teleport() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        WithTip(RunesmithHoverTip.Break);
        WithTips(static (card) =>
        {
            if (card.IsUpgraded) return [RunesmithHoverTipFactory.Static(RunesmithHoverTip.Charge)];
            return [];
        });
        WithVar(new ChargeGainVar(0, false).WithUpgrade(2));
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override RuneBreakType RuneBreakType => RuneBreakType.Newest;

    protected override bool ShouldGlowGoldInternal => HasRune();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var targetPlayer = play.Target?.Player;
        ArgumentNullException.ThrowIfNull(targetPlayer);
        if (HasRune())
        {
            var runes = Owner.PlayerCombatState?.GetRuneQueue()?.Runes;
            var runeOrig = runes?[^1];
            if (runeOrig == null) return;
            var runeCopy = (RuneModel)runeOrig.MutableClone();
            runeCopy.TransferOwner(targetPlayer);

            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

            await RuneCmd.Break(choiceContext, Owner, runeOrig);

            runeCopy.ChargeVal += DynamicVars[ChargeGainVar.defaultName].IntValue;

            await RuneCmd.AddRune(choiceContext, runeCopy, targetPlayer, play);
        }
    }
}