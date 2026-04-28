#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Runesonance : Runesmith2Card, IAfterRuneCrafted, IAfterRuneBroken
{
    public Runesonance() : base(4, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVar(new ChargeGainVar(4).WithUpgrade(2));
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        RuneCmd.ChargeAll(choiceContext, Owner, DynamicVars[ChargeGainVar.defaultName].IntValue);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return Task.CompletedTask;
        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue == null) return Task.CompletedTask;
        var count = runeQueue.Runes.Count;
        EnergyCost.AddThisTurn(-count);
        return Task.CompletedTask;
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this) return Task.CompletedTask;
        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue == null) return Task.CompletedTask;
        var count = runeQueue.Runes.Count;
        EnergyCost.AddThisTurn(-count);
        return Task.CompletedTask;
    }


    public Task AfterRuneCrafted(PlayerChoiceContext choiceContext, Player player, RuneModel rune)
    {
        if (player != Owner) return Task.CompletedTask;
        EnergyCost.AddThisTurn(-1);

        return Task.CompletedTask;
    }

    public Task AfterRuneBroken(PlayerChoiceContext choiceContext, Player player, RuneModel rune)
    {
        if (player != Owner) return Task.CompletedTask;
        EnergyCost.AddThisTurn(1);

        return Task.CompletedTask;
    }
}