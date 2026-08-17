#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Duplicate : Runesmith2Card
{
    public Duplicate() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(RunesmithHoverTip.Craft);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("IfFull", IsRuneSlotsFull());
    }

    protected override bool ShouldGlowRedInternal => IsRuneSlotsFull();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var runeQueue = Owner.PlayerCombatState?.GetRuneQueue();
        if (runeQueue != null && runeQueue.HasAny())
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            var clonedRune = runeQueue.Runes[^1].CreateClone();
            await RuneCmd.Craft(choiceContext, clonedRune, Owner, play, clonedRune.ChargeVal, clonedRune.PassiveVal,
                clonedRune.Upgraded);
        }
    }
}