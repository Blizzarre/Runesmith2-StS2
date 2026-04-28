#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Duplicate : Runesmith2Card
{
    public Duplicate() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(RunesmithHoverTip.Craft);
        WithCostUpgradeBy(-1);
    }

    public override Elements CanonicalElementsCost => new(1);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue != null)
        {
            // await RuneCmd.BreakOldest(choiceContext, Owner);
            // if (!runeQueue.HasAny())
            //     return;
            // await Cmd.CustomScaledWait(0.1f, 0.2f);
            var clonedRune = runeQueue.Runes[^1].CreateClone();
            await RuneCmd.Craft(choiceContext, clonedRune, Owner, play, clonedRune.ChargeVal, clonedRune.PassiveVal,
                clonedRune.Upgraded);
        }
    }
}