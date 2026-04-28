#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class AllInOne : Runesmith2Card
{
    public AllInOne() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(RunesmithHoverTip.Break);
        WithTip(RunesmithHoverTip.Charge);
        WithTip(RunesmithHoverTip.Potency);
        WithCostUpgradeBy(-1);
    }

    protected override bool ShouldGlowGoldInternal {
        get
        {
            var runeQueue = Owner.PlayerCombatState?.RuneQueue();
            return runeQueue is { Runes.Count: > 1 };
        }
    }

    public override RuneBreakType RuneBreakType => RuneBreakType.AllExceptNewest;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue is { Runes.Count: > 1 })
        {
            var brokenRunes = new List<RuneModel>();
            var runeToBreak = runeQueue.Runes.Count - 1;
            for (var i = 0; i < runeToBreak; i++)
            {
                var rune = await RuneCmd.BreakOldest(choiceContext, Owner);
                if (rune != null) brokenRunes.Add(rune);
                await Cmd.CustomScaledWait(0.1f, 0.2f);
            }

            var potencyCharge = brokenRunes.Select(r => (r.PassiveVal, r.ChargeVal))
                .Aggregate((a, b) => (a.PassiveVal + b.PassiveVal, a.ChargeVal + b.ChargeVal));

            await RuneCmd.AddPotency(choiceContext, runeQueue.Runes, Owner, play, potencyCharge.PassiveVal);
            RuneCmd.Charge(choiceContext, runeQueue.Runes, potencyCharge.ChargeVal);
        }
    }
}