#region

using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Runesonance : Runesmith2Card
{
    public Runesonance() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVar(new ChargeGainVar(1));
        WithVar(new DisplayVar<Runesonance>("ChargeSet", c =>
        {
            if (!c.IsInCombat) return "0";
            var runeQueue = c.Owner.PlayerCombatState?.RuneQueue();
            if (runeQueue is not { Runes.Count: > 0 }) return "0";
            var amount = runeQueue.Runes[^1].ChargeVal + (c.IsUpgraded ? c.DynamicVars[ChargeGainVar.defaultName].IntValue : 0);
            return $"{amount}";
        }));
    }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("HasRune", HasRune());
    }

    protected override bool ShouldGlowGoldInternal => HasRune();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue is { Runes.Count: > 0 })
        {
            var amount = runeQueue.Runes[^1].ChargeVal;
            foreach (var rune in runeQueue.Runes) RuneCmd.SetCharge(choiceContext, rune, amount);
            
            if (IsUpgraded)
                RuneCmd.ChargeAll(choiceContext, Owner, DynamicVars[ChargeGainVar.defaultName].IntValue);
        }
    }
}