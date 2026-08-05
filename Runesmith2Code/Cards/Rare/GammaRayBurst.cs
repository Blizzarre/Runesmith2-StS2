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

public class GammaRayBurst : Runesmith2Card
{
    public GammaRayBurst() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVar("Amount", 0, 1);
        WithTip(RunesmithHoverTip.Break);
        WithTip(RunesmithHoverTip.Charge);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var xValue = ResolveEnergyXValue() + DynamicVars["Amount"].IntValue;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        var runeQueue = Owner.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null || !runeQueue.HasAny()) return;
        
        await RuneCmd.PassiveAll(choiceContext, Owner, xValue);
        
        var index = 0;
        RuneModel? currRune = null;
        while (index < runeQueue.Runes.Count)
        {
            var nextRune = runeQueue.Runes[index];
            if (nextRune.ChargeVal == 0 && nextRune != currRune)
            {
                currRune = nextRune;
                await RuneCmd.Break(choiceContext, Owner, nextRune);
                await Cmd.CustomScaledWait(0.1f, 0.2f);
            }
            else
            {
                // increment index as rune wasn't broken
                index++;
            }
        }
    }
}